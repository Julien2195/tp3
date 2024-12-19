using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace TP2
{
    class Program {

        public static string connectionString = "Server=localhost;Trusted_Connection=True;Database=StoreDB;TrustServerCertificate=True;Encrypt=false;";
        static void Main(string[] args)
        {
            Init.InitializeDatabase();

            while (true)
            {
                Console.WriteLine("1. Ajouter un produit");
                Console.WriteLine("2. Modifier le prix d'un produit");
                Console.WriteLine("3. Supprimer un produit");
                Console.WriteLine("4. Afficher les produits par catégorie");
                Console.WriteLine("5. Afficher le prix cumulé de tous les produits");
                Console.WriteLine("6. Quitter");

                Console.Write("Choisissez une option : ");
                int choice = int.Parse(Console.ReadLine()!);

                switch (choice)
                {
                    case 1:
                        AjouterProduit();
                        break;
                    case 2:
                        ModifierPrixProduit();
                        break;
                    case 3:
                        SupprimerProduit();
                        break;
                    case 4:
                        AfficherProduitsParCategorie();
                        break;
                    case 5:
                        AfficherPrixCumulé();
                        break;
                    case 6:
                        return;
                    default:
                        Console.WriteLine("Option invalide");
                        break;
                }
            }
        }
        static void AjouterProduit()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                Console.Write("Nom du produit : ");
                string nom_pro = Console.ReadLine()!;
                Console.Write("Quantité : ");
                int quantite_pro = int.Parse(Console.ReadLine()!);
                Console.Write("Prix : ");
                decimal prix_pro = decimal.Parse(Console.ReadLine()!);
                Console.Write("ID de la catégorie : ");
                int fk_cat = int.Parse(Console.ReadLine()!);

                string query = "INSERT INTO Products (nom_pro, quantite_pro, prix_pro, fk_cat) VALUES (@nom_pro, @quantite_pro, @prix_pro, @fk_cat)";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@nom_pro", nom_pro);
                    cmd.Parameters.AddWithValue("@quantite_pro", quantite_pro);
                    cmd.Parameters.AddWithValue("@prix_pro", prix_pro);
                    cmd.Parameters.AddWithValue("@fk_cat", fk_cat);
                    cmd.ExecuteNonQuery();
                }

                Console.WriteLine("Produit ajouté avec succès.");
            }
        }

        static void ModifierPrixProduit()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                Console.Write("ID du produit à modifier : ");
                int id = int.Parse(Console.ReadLine());
                Console.Write("Nouveau prix : ");
                decimal newPrice = decimal.Parse(Console.ReadLine());

                string query = "UPDATE Products SET prix_pro = @prix_pro WHERE pk_pro = @pk_pro";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@prix_pro", newPrice);
                    cmd.Parameters.AddWithValue("@pk_pro", id);
                    cmd.ExecuteNonQuery();
                }

                Console.WriteLine("Prix du produit modifié avec succès.");
            }
        }

        static void SupprimerProduit()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                Console.Write("ID du produit à supprimer : ");
                int id = int.Parse(Console.ReadLine());

                string query = "DELETE FROM Products WHERE pk_pro = @pk_pro";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@pk_pro", id);
                    cmd.ExecuteNonQuery();
                }

                Console.WriteLine("Produit supprimé avec succès.");
            }
        }

        static void AfficherProduitsParCategorie()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                Console.Write("Nom de la catégorie : ");
                string category = Console.ReadLine();

                string query = @"
                    SELECT nom_pro, quantite_pro, prix_pro 
                    FROM Products 
                    INNER JOIN Categories ON Products.fk_cat = Categories.pk_cat 
                    WHERE nom_cat = @nom_cat";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@nom_cat", category);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        Console.WriteLine("Produits dans la catégorie " + category + ":");
                        while (reader.Read())
                        {
                            Console.WriteLine($"- {reader["nom_pro"]} : {reader["quantite_pro"]} unités, {reader["prix_pro"]} €");
                        }
                    }
                }
            }
        }

        static void AfficherPrixCumulé()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string query = "SELECT SUM(prix_pro * quantite_pro) AS PrixTotal FROM Products";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    decimal prixTotal = (decimal)cmd.ExecuteScalar();
                    Console.WriteLine($"Prix cumulé de tous les produits : {prixTotal} €");
                }
            }
        }
    }
}