using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ProductService.Data;

public class DbConnectionFactory
{
    private readonly string _connStr;
    private readonly string _masterConnStr;

    public DbConnectionFactory(IConfiguration config)
    {
        _connStr = config.GetConnectionString("DefaultConnection")!;

        var builder = new SqlConnectionStringBuilder(_connStr);
        builder.InitialCatalog = "master";
        builder.ConnectTimeout = 30;

        _masterConnStr = builder.ConnectionString;
    }

    public IDbConnection CreateConnection() => new SqlConnection(_connStr);

    public async Task InitializeAsync()
    {
        // Create Database
        using (var masterConn = new SqlConnection(_masterConnStr))
        {
            await masterConn.OpenAsync();

            await masterConn.ExecuteAsync(@"
                IF NOT EXISTS (
                    SELECT name 
                    FROM sys.databases 
                    WHERE name = 'ShopEz_Products'
                )
                CREATE DATABASE [ShopEz_Products];
            ");
        }

        await Task.Delay(2000);

        using var conn = new SqlConnection(_connStr);

        await conn.OpenAsync();

        // Create Products Table
        await conn.ExecuteAsync(@"

            IF NOT EXISTS (
                SELECT * 
                FROM sysobjects 
                WHERE name='Products' AND xtype='U'
            )

            CREATE TABLE Products (
                 Id INT IDENTITY(1,1) PRIMARY KEY,
                Name NVARCHAR(200) NOT NULL,
                [Desc] NVARCHAR(1000) NOT NULL,
                Price DECIMAL(18,2) NOT NULL,
                Stock INT NOT NULL,
                ImageUrl NVARCHAR(1000) NOT NULL,
                Category NVARCHAR(100) NOT NULL
            );

            IF NOT EXISTS (SELECT 1 FROM Products)
            BEGIN

                INSERT INTO Products
                ( Name, [Desc], Price, Stock, ImageUrl, Category)

                VALUES

                (
                    
                    'Sony WH-1000XM5',
                    'Industry-leading noise cancellation, 30hr battery, crystal clear audio.',
                    24999,
                    15,
                    'https://images.unsplash.com/photo-1505740420928-5e560c06d30e?w=400',
                    'Headphones'
                ),

                (
                    
                    'Apple AirPods Pro 2nd Gen',
                    'Active noise cancellation, transparency mode and spatial audio.',
                    19999,
                    20,
                    'https://images.unsplash.com/photo-1600294037681-c80b4cb5b434?w=400',
                    'Headphones'
                ),

                (
                    
                    'JBL Tune 760NC',
                    'Wireless over-ear headphones with ANC and 35hr battery life.',
                    5999,
                    25,
                    'https://images.unsplash.com/photo-1484704849700-f032a568e944?w=400',
                    'Headphones'
                ),

                (
                    
                    'iPhone 15 Pro Max',
                    'A17 Pro chip, titanium design, 48MP camera with 5x optical zoom.',
                    134900,
                    10,
                    'https://images.unsplash.com/photo-1695048133142-1a20484d2569?w=400',
                    'Smartphones'
                ),

                (
                    
                    'Samsung Galaxy S24 Ultra',
                    'Snapdragon 8 Gen 3, 200MP camera, built-in S Pen and AI features.',
                    129999,
                    8,
                    'https://images.unsplash.com/photo-1706439154236-f5a4b0763d61?w=400',
                    'Smartphones'
                ),

                (
                    
                    'OnePlus 12',
                    'Snapdragon 8 Gen 3, Hasselblad camera, 100W SUPERVOOC charging.',
                    64999,
                    15,
                    'https://images.unsplash.com/photo-1598327105666-5b89351aff97?w=400',
                    'Smartphones'
                ),

                (
                
                    'MacBook Pro 14 M3 Pro',
                    'Apple M3 Pro chip, Liquid Retina XDR display, 18hr battery.',
                    199900,
                    8,
                    'https://images.unsplash.com/photo-1517336714731-489689fd1ca8?w=400',
                    'Laptops'
                ),

                (
                    
                    'Dell XPS 15 OLED',
                    'Intel Core i9, RTX 4070, OLED touchscreen display.',
                    159999,
                    6,
                    'https://images.unsplash.com/photo-1593642632559-0c6d3fc62b89?w=400',
                    'Laptops'
                ),

                (
                    
                    'ASUS ROG Zephyrus G14',
                    'AMD Ryzen 9 gaming laptop.',
                    119999,
                    10,
                    'https://images.unsplash.com/photo-1525547719571-a2d4ac8945e2?w=400',
                    'Laptops'
                ),

                (
                    
                    'iPad Pro 12.9 M2',
                    'Apple M2 tablet with Liquid Retina XDR.',
                    99900,
                    10,
                    'https://images.unsplash.com/photo-1544244015-0df4b3ffc6b0?w=400',
                    'Tablets'
                )
END
        ");
    }
}