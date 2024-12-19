using System;
using Microsoft.Data.SqlClient;

namespace TP2
{
    public static class Init
    {
        public static void InitializeDatabase()
        {
            string connectionString = "Server=localhost;Trusted_Connection=True;Database=master;TrustServerCertificate=True;Encrypt=false;";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string createDb = "IF DB_ID('StoreDB') IS NULL CREATE DATABASE StoreDB;";
                using (SqlCommand cmd = new SqlCommand(createDb, conn))
                {
                    try
                    {
                        cmd.ExecuteNonQuery();
                    }
                    catch (SqlException ex)
                    {
                        Console.WriteLine("Erreur lors de la création de la base de données : " + ex.Message);
                    }
                }
                conn.ChangeDatabase("StoreDB");

                string createTablesQuery = @"
                    IF OBJECT_ID('Categories', 'U') IS NULL
                    CREATE TABLE Categories (
                        pk_cat INT PRIMARY KEY IDENTITY,
                        nom_cat NVARCHAR(100) NOT NULL
                    );

                    IF OBJECT_ID('Products', 'U') IS NULL
                    CREATE TABLE Products (
                        pk_pro INT PRIMARY KEY IDENTITY,
                        nom_pro NVARCHAR(100) NOT NULL,
                        quantite_pro INT NOT NULL,
                        prix_pro DECIMAL(18, 2) NOT NULL,
                        fk_cat INT NOT NULL FOREIGN KEY REFERENCES Categories(pk_cat)
                    );
                ";
                using (SqlCommand cmd = new SqlCommand(createTablesQuery, conn))
                {
                    cmd.ExecuteNonQuery();
                }

                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        string insertCategoriesQuery = @"
                            IF NOT EXISTS (SELECT 1 FROM Categories)
                            BEGIN
                                INSERT INTO Categories (nom_cat) VALUES (@cat1);
                                INSERT INTO Categories (nom_cat) VALUES (@cat2);
                                INSERT INTO Categories (nom_cat) VALUES (@cat3);
                            END;
                        ";

                        using (SqlCommand cmd = new SqlCommand(insertCategoriesQuery, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@cat1", "Informatique");
                            cmd.Parameters.AddWithValue("@cat2", "Cuisine");
                            cmd.Parameters.AddWithValue("@cat3", "Sport");
                            cmd.ExecuteNonQuery();
                        }

                        string insertProductsQuery = @"
                            IF NOT EXISTS (SELECT 1 FROM Products)
                            BEGIN
                                INSERT INTO Products (nom_pro, quantite_pro, prix_pro, fk_cat)
                                VALUES (@nom1, @quant1, @prix1, @fk1),
                                       (@nom2, @quant2, @prix2, @fk2),
                                       (@nom3, @quant3, @prix3, @fk3),
                                       (@nom4, @quant4, @prix4, @fk4),
                                       (@nom5, @quant5, @prix5, @fk5),
                                       (@nom6, @quant6, @prix6, @fk6);
                            END;
                        ";

                        using (SqlCommand cmd = new SqlCommand(insertProductsQuery, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@nom1", "produit1");
                            cmd.Parameters.AddWithValue("@quant1", 1);
                            cmd.Parameters.AddWithValue("@prix1",10);
                            cmd.Parameters.AddWithValue("@fk1", 1);

                            cmd.Parameters.AddWithValue("@nom2", "produit2");
                            cmd.Parameters.AddWithValue("@quant2", 5);
                            cmd.Parameters.AddWithValue("@prix2", 20);
                            cmd.Parameters.AddWithValue("@fk2", 2);

                            cmd.Parameters.AddWithValue("@nom3", "Produit3");
                            cmd.Parameters.AddWithValue("@quant3", 20);
                            cmd.Parameters.AddWithValue("@prix3", 30);
                            cmd.Parameters.AddWithValue("@fk3", 1);

                            cmd.Parameters.AddWithValue("@nom4", "Produit4");
                            cmd.Parameters.AddWithValue("@quant4", 15);
                            cmd.Parameters.AddWithValue("@prix4", 60);
                            cmd.Parameters.AddWithValue("@fk4", 2);

                            cmd.Parameters.AddWithValue("@nom5", "Peroduit5");
                            cmd.Parameters.AddWithValue("@quant5", 30);
                            cmd.Parameters.AddWithValue("@prix5", 12);
                            cmd.Parameters.AddWithValue("@fk5", 3);

                            cmd.Parameters.AddWithValue("@nom6", "Produit6");
                            cmd.Parameters.AddWithValue("@quant6", 50);
                            cmd.Parameters.AddWithValue("@prix6", 40);
                            cmd.Parameters.AddWithValue("@fk6", 3);

                            cmd.ExecuteNonQuery();
                        }

                        transaction.Commit();
                        Console.WriteLine("Base de données initialisée avec succès.");
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Console.WriteLine("Erreur lors de l'initialisation de la base de données : " + ex.Message);
                    }
                }
            }
        }
    }
}
