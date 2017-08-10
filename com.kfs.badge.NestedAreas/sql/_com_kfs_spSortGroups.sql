CREATE PROC [dbo].[_com_kfs_spSortGroups](@GroupTypeId INT)
AS
     DECLARE @SortedTable TABLE
     (Id        INT,
      SortOrder INT
     );
     WITH grouptree(Root,
                    Parent,
                    Name,
                    Level,
                    ID,
                    Branch)
          AS (
          SELECT [Name] AS Root,
                 CAST('' AS NVARCHAR(100)) AS Parent,
                 [Name] AS Name,
                 CAST('0' AS INT) AS Level,
                 Id AS ID,
                 CAST(Id AS VARCHAR) AS Branch
          FROM [Group]
          WHERE(ParentGroupId IS NULL)
               AND (GroupTypeId = @GroupTypeId)
          UNION ALL
          SELECT tree.Root,
                 tree.[Name] AS Parent,
                 CONVERT(NVARCHAR(100), REPLICATE('-', tree.level+1)+[Group].[Name]) AS Name,
                 tree.Level + 1 AS Expr1,
                 [Group].Id,
                 CAST(tree.Branch+'.'+CAST([Group].Id AS VARCHAR) AS VARCHAR) AS Expr2
          FROM [Group]
               INNER JOIN grouptree AS tree ON [Group].ParentGroupId = tree.id)
          INSERT INTO @SortedTable
                 SELECT Id,
                        ROW_NUMBER() OVER(ORDER BY Root,
                                                   Branch ASC) AS SortOrder
                 FROM grouptree AS PT
                 ORDER BY Root,
                          Branch;
     UPDATE g
       SET
           g.[Order] = s.SortOrder
     FROM [Group] g
          JOIN @SortedTable s ON g.Id = s.Id;
GO


