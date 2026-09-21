// <copyright>
// Copyright 2026 by Kingdom First Solutions
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
//
using Rock;
using Rock.Plugin;

namespace rocks.kfs.MobileAppMigration.Migrations
{
    /// <summary>
    /// "Add to Home Screen" for the Nfluence Church mobile application.
    ///
    /// Anything carrying an AddToHomeScreen attribute set to True gets a card on
    /// the App Home Feed; un-flagging expires the card. Handles all four Content
    /// Source types - content channel items, event items, groups and registration
    /// instances.
    ///
    /// Installs two things:
    ///   1. the dbo._rocks_kfs_SyncHomeScreenCards stored procedure
    ///   2. an hourly Rock.Jobs.RunSQL job whose whole SQL is one EXEC line
    ///
    /// IT DOES NOT CREATE THE AddToHomeScreen TOGGLES.
    /// The toggle belongs on whatever content a given church wants taggable -
    /// Messages, Series and Notes here - and those channels come from that
    /// church's import, not from this plugin. A migration should not attach
    /// attributes to content it does not own. Add them per server with
    /// ADD_HomeScreenToggle.sql, or through Rock's UI for groups, event calendars
    /// and registration templates. The job finds them by Key, so it needs no
    /// changing when one is added.
    ///
    /// WHY A STORED PROCEDURE RATHER THAN THE SQL IN THE JOB
    /// The sync is ~8,000 characters. Pasted into the Run SQL job's textarea it was
    /// silently cut off at 7,680 - everything from the card-creating INSERT onward
    /// was lost. The job then reported Success in 0 seconds and created nothing,
    /// because the surviving half still parsed. Keeping the body in a procedure
    /// makes the job's SQL a single line that cannot be truncated.
    ///
    /// The procedure body is generated from JOB_SyncHomeScreenCards.sql - edit that
    /// file and re-run the generator rather than editing this one, or the two will
    /// drift.
    ///
    /// Run AFTER migration 1 (the app itself) - it needs the App Home Feed channel
    /// and its Content Source attributes.
    /// </summary>
    [MigrationNumber( 3, "1.16.0" )]
    public class NfluenceHomeScreenSync : Migration
    {
        private const string JobGuid = "71F19F08-5834-41CC-A274-1D0AB8A06378";
        private const string SqlQueryAttributeGuid = "7AD0C57A-D40E-4A14-81D8-8ACA68600FF5";
        private const string CommandTimeoutAttributeGuid = "FF66ABF1-B01D-4AE7-814E-95D842B2EA99";

        public override void Up()
        {
            //
            // 1. The sync procedure. DROP and CREATE are separate calls because
            //    CREATE PROCEDURE must be the first statement in its batch and
            //    Sql() sends one batch per call.
            //
            Sql( @"IF OBJECT_ID( 'dbo._rocks_kfs_SyncHomeScreenCards', 'P' ) IS NOT NULL DROP PROCEDURE dbo._rocks_kfs_SyncHomeScreenCards;" );

            Sql( @"CREATE PROCEDURE dbo._rocks_kfs_SyncHomeScreenCards
AS
BEGIN
SET NOCOUNT ON;

DECLARE @Now      DATETIME      = GETDATE();
DECLARE @Prefix   NVARCHAR(20)  = 'HomeScreenSync:';
DECLARE @FeedGuid UNIQUEIDENTIFIER = '122CAAAE-0698-4869-89FE-D818E109BAEA';

DECLARE @FeedChannelId INT = ( SELECT TOP 1 [Id] FROM [ContentChannel] WHERE [Guid] = @FeedGuid );
IF @FeedChannelId IS NULL
BEGIN
    SELECT 'App Home Feed channel not on this server - nothing to do.' AS [Result];
    RETURN;
END

DECLARE @FeedTypeId INT = ( SELECT TOP 1 [ContentChannelTypeId] FROM [ContentChannel] WHERE [Id] = @FeedChannelId );

/* ---- the feed's Content Source attributes, by Key -------------------------- */
DECLARE @FeedAttr TABLE ( AttrKey NVARCHAR(50), AttrId INT );
INSERT INTO @FeedAttr ( AttrKey, AttrId )
SELECT a.[Key], a.[Id] FROM [Attribute] a
WHERE a.[EntityTypeQualifierColumn] = 'ContentChannelId'
  AND a.[EntityTypeQualifierValue] = CAST( @FeedChannelId AS NVARCHAR(20) )
  AND a.[Key] IN ( 'LinkedItem', 'EventItem', 'LinkedGroup', 'RegistrationInstance' );

/* ---- everything currently flagged ----------------------------------------- */
DECLARE @Wanted TABLE ( SourceGuid UNIQUEIDENTIFIER, FeedAttrKey NVARCHAR(50),
                        SourceName NVARCHAR(250), SourceExpire DATETIME, FK NVARCHAR(100) );

DECLARE @EtEvent INT = ( SELECT TOP 1 [Id] FROM [EntityType] WHERE [Name] = 'Rock.Model.EventItem' );
DECLARE @EtGroup INT = ( SELECT TOP 1 [Id] FROM [EntityType] WHERE [Name] = 'Rock.Model.Group' );
DECLARE @EtReg   INT = ( SELECT TOP 1 [Id] FROM [EntityType] WHERE [Name] = 'Rock.Model.RegistrationInstance' );

/* content channel items - the feed channel itself is excluded, or a flagged card
   could point at itself */
INSERT INTO @Wanted ( SourceGuid, FeedAttrKey, SourceName, SourceExpire )
SELECT i.[Guid], 'LinkedItem', i.[Title], i.[ExpireDateTime]
FROM [ContentChannelItem] i
JOIN [Attribute] a ON a.[Key] = 'AddToHomeScreen'
                  AND a.[EntityTypeQualifierColumn] = 'ContentChannelId'
                  AND a.[EntityTypeQualifierValue] = CAST( i.[ContentChannelId] AS NVARCHAR(20) )
JOIN [AttributeValue] av ON av.[AttributeId] = a.[Id] AND av.[EntityId] = i.[Id]
WHERE av.[Value] = 'True'
  AND i.[ContentChannelId] <> @FeedChannelId;

/* event items */
INSERT INTO @Wanted ( SourceGuid, FeedAttrKey, SourceName, SourceExpire )
SELECT e.[Guid], 'EventItem', e.[Name], NULL
FROM [EventItem] e
JOIN [Attribute] a ON a.[EntityTypeId] = @EtEvent AND a.[Key] = 'AddToHomeScreen'
JOIN [AttributeValue] av ON av.[AttributeId] = a.[Id] AND av.[EntityId] = e.[Id]
WHERE av.[Value] = 'True' AND e.[IsActive] = 1;

/* groups - honour the GroupTypeId qualifier when there is one */
INSERT INTO @Wanted ( SourceGuid, FeedAttrKey, SourceName, SourceExpire )
SELECT g.[Guid], 'LinkedGroup', g.[Name], NULL
FROM [Group] g
JOIN [Attribute] a ON a.[EntityTypeId] = @EtGroup AND a.[Key] = 'AddToHomeScreen'
                  AND ( ISNULL( a.[EntityTypeQualifierColumn], '' ) = ''
                        OR ( a.[EntityTypeQualifierColumn] = 'GroupTypeId'
                             AND a.[EntityTypeQualifierValue] = CAST( g.[GroupTypeId] AS NVARCHAR(20) ) ) )
JOIN [AttributeValue] av ON av.[AttributeId] = a.[Id] AND av.[EntityId] = g.[Id]
WHERE av.[Value] = 'True' AND g.[IsActive] = 1 AND g.[IsArchived] = 0;

/* registration instances - registration closing date becomes the card's expiry */
INSERT INTO @Wanted ( SourceGuid, FeedAttrKey, SourceName, SourceExpire )
SELECT ri.[Guid], 'RegistrationInstance', ri.[Name], ri.[EndDateTime]
FROM [RegistrationInstance] ri
JOIN [Attribute] a ON a.[EntityTypeId] = @EtReg AND a.[Key] = 'AddToHomeScreen'
                  AND ( ISNULL( a.[EntityTypeQualifierColumn], '' ) = ''
                        OR ( a.[EntityTypeQualifierColumn] = 'RegistrationTemplateId'
                             AND a.[EntityTypeQualifierValue] = CAST( ri.[RegistrationTemplateId] AS NVARCHAR(20) ) ) )
JOIN [AttributeValue] av ON av.[AttributeId] = a.[Id] AND av.[EntityId] = ri.[Id]
WHERE av.[Value] = 'True' AND ri.[IsActive] = 1;

UPDATE @Wanted SET FK = @Prefix + LOWER( CAST( SourceGuid AS NVARCHAR(50) ) );

/* drop anything whose Content Source attribute does not exist on this server,
   and any accidental duplicate of the same source */
DELETE w FROM @Wanted w
 WHERE NOT EXISTS ( SELECT 1 FROM @FeedAttr fa WHERE fa.AttrKey = w.FeedAttrKey );

;WITH dupes AS ( SELECT ROW_NUMBER() OVER ( PARTITION BY FK ORDER BY SourceName ) AS rn FROM @Wanted )
DELETE FROM dupes WHERE rn > 1;

/* ---- 1. expire the cards whose flag went off, or whose source is gone ------ */
UPDATE i
   SET i.[ExpireDateTime] = @Now, i.[ModifiedDateTime] = @Now
FROM [ContentChannelItem] i
WHERE i.[ContentChannelId] = @FeedChannelId
  AND i.[ForeignKey] LIKE @Prefix + '%'
  AND ( i.[ExpireDateTime] IS NULL OR i.[ExpireDateTime] > @Now )
  AND NOT EXISTS ( SELECT 1 FROM @Wanted w WHERE w.FK = i.[ForeignKey] );

DECLARE @Expired INT = @@ROWCOUNT;

/* ---- 2. revive a card that was flagged again ------------------------------ */
UPDATE i
   SET i.[ExpireDateTime] = w.SourceExpire, i.[ModifiedDateTime] = @Now
FROM [ContentChannelItem] i
JOIN @Wanted w ON w.FK = i.[ForeignKey]
WHERE i.[ContentChannelId] = @FeedChannelId
  AND i.[ExpireDateTime] IS NOT NULL
  AND i.[ExpireDateTime] <= @Now;

DECLARE @Revived INT = @@ROWCOUNT;

/* ---- 3. create cards for anything newly flagged --------------------------- */
DECLARE @BaseOrder INT = ISNULL( ( SELECT MAX( [Order] ) FROM [ContentChannelItem]
                                    WHERE [ContentChannelId] = @FeedChannelId ), 0 );

DECLARE @New TABLE ( ItemId INT, FK NVARCHAR(100) );

INSERT INTO [ContentChannelItem]
    ( [ContentChannelId], [ContentChannelTypeId], [Title], [Content], [Priority], [Order],
      [Status], [ApprovedDateTime], [StartDateTime], [ExpireDateTime],
      [Guid], [ForeignKey], [CreatedDateTime], [ModifiedDateTime] )
OUTPUT inserted.[Id], inserted.[ForeignKey] INTO @New ( ItemId, FK )
SELECT @FeedChannelId, @FeedTypeId, LEFT( ISNULL( w.SourceName, '' ), 250 ), '', 0,
       @BaseOrder + ROW_NUMBER() OVER ( ORDER BY w.SourceName ),
       2, @Now, @Now, w.SourceExpire,
       NEWID(), w.FK, @Now, @Now
FROM @Wanted w
WHERE NOT EXISTS ( SELECT 1 FROM [ContentChannelItem] i
                    WHERE i.[ContentChannelId] = @FeedChannelId AND i.[ForeignKey] = w.FK )
  /* and no card already points at this source, including one made by hand */
  AND NOT EXISTS ( SELECT 1
                     FROM [AttributeValue] av2
                     JOIN @FeedAttr fa2 ON fa2.AttrId = av2.[AttributeId] AND fa2.AttrKey = w.FeedAttrKey
                     JOIN [ContentChannelItem] i2 ON i2.[Id] = av2.[EntityId]
                                                 AND i2.[ContentChannelId] = @FeedChannelId
                    WHERE LOWER( CAST( av2.[Value] AS NVARCHAR(50) ) )
                        = LOWER( CAST( w.SourceGuid AS NVARCHAR(50) ) ) );

DECLARE @Created INT = @@ROWCOUNT;

/* ---- 4. point each new card at its source --------------------------------- */
INSERT INTO [AttributeValue] ( [IsSystem], [AttributeId], [EntityId], [Value], [Guid],
                               [CreatedDateTime], [ModifiedDateTime] )
SELECT 0, fa.AttrId, n.ItemId, LOWER( CAST( w.SourceGuid AS NVARCHAR(50) ) ), NEWID(), @Now, @Now
FROM @New n
JOIN @Wanted w ON w.FK = n.FK
JOIN @FeedAttr fa ON fa.AttrKey = w.FeedAttrKey;

/* ---- what happened -------------------------------------------------------- */
SELECT CAST( @Created AS VARCHAR(10) ) + ' created, '
     + CAST( @Revived AS VARCHAR(10) ) + ' revived, '
     + CAST( @Expired AS VARCHAR(10) ) + ' expired; '
     + CAST( ( SELECT COUNT(*) FROM @Wanted ) AS VARCHAR(10) ) + ' flagged, '
     + CAST( ( SELECT COUNT(*) FROM [ContentChannelItem]
                WHERE [ContentChannelId] = @FeedChannelId
                  AND [ForeignKey] LIKE @Prefix + '%'
                  AND ( [ExpireDateTime] IS NULL OR [ExpireDateTime] > @Now ) ) AS VARCHAR(10) )
     + ' live sync cards' AS [Result];
END" );

            //
            // 2. The hourly job.
            //
            //    Written out rather than using RockMigrationHelper.AddPostUpdateServiceJob:
            //    that helper forces IsSystem = 1 and exists for one-shot post-update
            //    jobs run at Rock start up. This is a recurring job a church admin
            //    should be able to see, retime, disable or delete, so it is created
            //    with IsSystem = 0.
            //
            Sql( $@"
                IF NOT EXISTS ( SELECT 1 FROM [ServiceJob] WHERE [Guid] = '{JobGuid}' )
                    INSERT INTO [ServiceJob] ( [IsSystem], [IsActive], [Name], [Description], [Class],
                                               [CronExpression], [NotificationStatus], [EnableHistory],
                                               [HistoryCount], [Guid] )
                    VALUES ( 0, 1, 'Auto Add App Home Feed Items',
                             'Creates an App Home Feed card for anything flagged Add to Home Screen, and expires the card when the flag is turned off. Only ever touches cards it created - those carry ForeignKey HomeScreenSync:<source guid> - so cards made by hand are never altered.',
                             'Rock.Jobs.RunSQL', '0 0 * * * ?', 1, 0, 500, '{JobGuid}' );
                " );

            // The job's entire SQL. This helper deletes any existing value first, so
            // it is safe to re-run and will correct a hand-edited or truncated job.
            RockMigrationHelper.AddServiceJobAttributeValue( JobGuid, SqlQueryAttributeGuid,
                "EXEC dbo._rocks_kfs_SyncHomeScreenCards;" );
        }

        public override void Down()
        {
            Sql( $@"
                DELETE av FROM [AttributeValue] av
                 WHERE av.[AttributeId] IN ( SELECT [Id] FROM [Attribute]
                                              WHERE [Guid] IN ( '{SqlQueryAttributeGuid}', '{CommandTimeoutAttributeGuid}' ) )
                   AND av.[EntityId] IN ( SELECT [Id] FROM [ServiceJob] WHERE [Guid] = '{JobGuid}' );

                DELETE FROM [ServiceJob] WHERE [Guid] = '{JobGuid}';
                " );

            Sql( @"IF OBJECT_ID( 'dbo._rocks_kfs_SyncHomeScreenCards', 'P' ) IS NOT NULL DROP PROCEDURE dbo._rocks_kfs_SyncHomeScreenCards;" );

            //
            // Any AddToHomeScreen toggles are left alone - this migration did not
            // create them, and dropping them would throw away every editor's flag.
            //
        }
    }
}
