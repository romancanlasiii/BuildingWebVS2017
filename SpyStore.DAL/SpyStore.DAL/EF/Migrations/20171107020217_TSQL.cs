using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace SpyStore.DAL.EF.Migrations
{
    public partial class TSQL : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
			string sql = "CREATE FUNCTION Store.GetOrderTotal (@OrderId INT) " +
				"RETURNs MONEY WITH SCHEMABINDING " +
				"BEGIN " +
					"DECLARE @Result MONEY; " +
					"SELECT @Result = SUM([Quantity] * [UnitCost]) FROM Store.OrderDetails " +
						"WHERE OrderId = @OrderId; " +
					"RETURN @Result " +
				"END";
			migrationBuilder.Sql(sql);

			sql = "CREATE PROCEDURE Store.PurchaseItemsInCart " +
				"(@CustomerId INT = 0, @OrderId INT OUTPUT) AS " +
				"BEGIN " +
					"SET NOCOUNT ON; " +
					"INSERT INTO Store.Orders (CustomerId, OrderDate, ShipDate) " +
						"VALUES (@CustomerId, GETDATE(), GETDATE()); " +
					"SET @OrderId = SCOPE_IDENTITY(); " +
					"DECLARE @TranName VARCHAR(20); " +
					"SELECT @TranName = 'CommitOrder'; " +
					"BEGIN TRANSACTION @TranName; " +
					"BEGIN TRY " +
						"INSERT INTO Store.OrderDetails (OrderId, ProductId, Quantity, UnitCost) " +
						"SELECT @OrderId, ProductId, Quantity, p.CurrentPrice " +
						"FROM Store.ShoppingCartRecords scr " +
							"INNER JOIN Store.Products p ON p.Id = scr.ProductId " +
						"WHERE CustomerId = @CustomerId; " +
						"DELETE FROM Store.ShoppingCartRecords WHERE CustomerId = @CustomerId; " +
						"COMMIT TRANSACTION @TranName; " +
					"END TRY " +
					"BEGIN CATCH " +
						"ROLLBACK TRANSACTION @TranName; " +
						"SET @OrderId = -1; " +
					"END CATCH; " +
				"END;";
			migrationBuilder.Sql(sql);
		}

        protected override void Down(MigrationBuilder migrationBuilder)
        {
			migrationBuilder.Sql("DROP FUNCTION Store.GetOrderTotal");
			migrationBuilder.Sql("DROP PROCEDURE Store.PurchaseItemsInCart");
        }
    }
}
