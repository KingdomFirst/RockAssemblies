// <copyright>
// Copyright 2024 by Kingdom First Solutions
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

namespace rocks.kfs.StepsToCare.Migrations
{
    [MigrationNumber( 17, "1.14.0" )]
    public class AddColumn : Migration
    {
        public override void Up()
        {
            Sql( @"IF COL_LENGTH('_rocks_kfs_StepsToCare_AssignedPerson', 'Type') IS NULL
                BEGIN
	                ALTER TABLE _rocks_kfs_StepsToCare_AssignedPerson ADD [Type] int NULL 
                END
                IF COL_LENGTH('_rocks_kfs_StepsToCare_AssignedPerson', 'TypeQualifier') IS NULL
                BEGIN
	                ALTER TABLE _rocks_kfs_StepsToCare_AssignedPerson ADD [TypeQualifier] [nvarchar](200) NULL
                END" );

            Sql( @"IF NOT EXISTS (SELECT Id FROM [_rocks_kfs_StepsToCare_NoteTemplate])
                BEGIN
                INSERT [_rocks_kfs_StepsToCare_NoteTemplate] ([Icon], [Note], [IsActive], [IsSystem], [Order], [Guid], [CreatedDateTime], [ModifiedDateTime], [CreatedByPersonAliasId], [ModifiedByPersonAliasId], [ForeignKey], [ForeignGuid], [ForeignId]) 
                VALUES
                (N'fas fa-phone-square', N'Called', 1, 0, 0, N'0f4eb2ba-5172-4426-97ea-d073e4a01c17', CAST(N'2021-08-16T16:49:25.657' AS DateTime), CAST(N'2022-01-12T16:41:20.283' AS DateTime), 10, 10, NULL, NULL, NULL)
                ,(N'far fa-comment', N'Text Message Sent', 1, 0, 1, N'66a18d82-2adc-4a9d-8efb-294608944caa', CAST(N'2021-08-16T17:10:18.933' AS DateTime), CAST(N'2022-01-12T16:41:20.283' AS DateTime), 10, 10, NULL, NULL, NULL)
                ,(N'fa fa-envelope', N'Mail Sent', 1, 0, 2, N'0aeed40a-dc09-40bf-a632-d42bcabd7279', CAST(N'2021-08-16T16:34:23.917' AS DateTime), CAST(N'2021-09-28T12:12:35.027' AS DateTime), 10, 10, NULL, NULL, NULL)
                ,(N'fas fa-gift', N'Gift Given', 1, 0, 3, N'c39103ff-026f-4930-ab77-2f163a2253e7', CAST(N'2021-08-16T17:11:10.687' AS DateTime), CAST(N'2021-09-28T12:08:36.340' AS DateTime), 10, 10, NULL, NULL, NULL)
                ,(N'fa fa-car', N'Visited', 1, 0, 4, N'5cec5c30-b512-4052-831f-9f510196debb', CAST(N'2022-09-07T20:47:53.187' AS DateTime), CAST(N'2022-09-07T20:47:53.187' AS DateTime), 10, 10, NULL, NULL, NULL)
                END" );
        }

        public override void Down()
        {
            Sql( @"IF COL_LENGTH('_rocks_kfs_StepsToCare_AssignedPerson', 'Type') IS NOT NULL
                BEGIN
	                ALTER TABLE _rocks_kfs_StepsToCare_AssignedPerson DROP COLUMN [Type]
                END
                IF COL_LENGTH('_rocks_kfs_StepsToCare_AssignedPerson', 'TypeQualifier') IS NOT NULL
                BEGIN
	                ALTER TABLE _rocks_kfs_StepsToCare_AssignedPerson DROP COLUMN [TypeQualifier]
                END" );

            // Would not delete note templates, you can manually do it if you wish.
        }
    }
}