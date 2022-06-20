using Transacoes_blockchain.domain;
using System;
using System.Data;
using System.Data.SQLite;

namespace Transacoes_blockchain.infra.data
{
  public class DalHelper
  {
    private static SQLiteConnection sqliteConnection;

    public DalHelper()
    { }

    private static SQLiteConnection DbConnection()
    {
      sqliteConnection = new SQLiteConnection("Data Source=TransacoesDatabase.sqlite;Version=3;");
      sqliteConnection.Open();
      return sqliteConnection;
    }

    public static void CriarBancoSQLite()
    {
      try
      {
        if (File.Exists("TransacoesDatabase.sqlite"))
        {
          return;
        }
        SQLiteConnection.CreateFile("TransacoesDatabase.sqlite");
      }
      catch
      {
        throw;
      }
    }

    public static void CriarTabelaTransacoesSQlite()
    {
      try
      {
        string sql = $"CREATE TABLE IF NOT EXISTS Transacoes (id int, remetente int, recebedor int, valor int, status int)";
        var connection = DbConnection();
        SQLiteCommand cmd = new (sql, connection);
        cmd.ExecuteNonQuery();
        connection.Close();
      }
      catch
      {
        throw;
      }
    }

    public static void CriarTabelaValidadoresSQlite()
    {
      try
      {
        string sql = $"CREATE TABLE IF NOT EXISTS Validadores (id int, nome Varchar(20), ip Varchar(20))";
        var connection = DbConnection();
        SQLiteCommand cmd = new(sql, connection);
        cmd.ExecuteNonQuery();
        connection.Close();
      }
      catch
      {
        throw;
      }
    }

    public static List<Transacao> GetTransacoes()
    {
      try
      {
        string sql = $"SELECT * FROM Transacoes";
        var connection = DbConnection();
        SQLiteCommand cmd = new(sql, connection);
        SQLiteDataReader dr = cmd.ExecuteReader();

        List<Transacao> list = new();
        while (dr.Read())
        {
          var transacao = new Transacao
          {
            Id = dr.GetInt32(0),
            Remetente = dr.GetInt32(1),
            Recebedor = dr.GetInt32(2),
            Valor = dr.GetInt32(3),
            Status = dr.GetInt32(4)
          };
          list.Add(transacao);
        }
        connection.Close();
        return list;
      }
      catch
      {
        throw;
      }
    }

    public static List<Validador> GetValidadores()
    {
      try
      {
        string sql = $"SELECT * FROM Validadores";
        var connection = DbConnection();
        SQLiteCommand cmd = new(sql, connection);
        SQLiteDataReader dr = cmd.ExecuteReader();

        List<Validador> list = new();
        while (dr.Read())
        {
          var validador = new Validador
          {
            Id = dr.GetInt32(0),
            Nome = dr.GetString(1),
            Ip = dr.GetString(2)
          };
          list.Add(validador);
        }
        connection.Close();
        return list;
      }
      catch
      {
        throw;
      }
    }

    public static Transacao GetById(string table, int id)
    {
      try
      {
        string sql = $"SELECT * FROM {table} Where Id={id}";
        var connection = DbConnection();
        SQLiteCommand cmd = new(sql, connection);
        SQLiteDataReader dr = cmd.ExecuteReader();

        var transacao = new Transacao();
        while (dr.Read())
        {
          transacao.Id = dr.GetInt32(0);
          transacao.Remetente = dr.GetInt32(1);
          transacao.Recebedor = dr.GetInt32(2);
          transacao.Valor = dr.GetInt32(3);
          transacao.Status = dr.GetInt32(4);
        }
        connection.Close();
        return transacao;
      }
      catch
      {
        throw;
      }
    }

    public static void DeleteById(string table, int id)
    {
      try
      {
        string sql = $"DELETE FROM {table} Where Id={id}";
        var connection = DbConnection();
        SQLiteCommand cmd = new(sql, connection);
        cmd.ExecuteNonQuery();
        connection.Close();
      }
      catch
      {
        throw;
      }
    }

    public static void AddTransacao(Transacao transacao)
    {
      try
      {
        string sql = $"INSERT INTO Transacoes (id, remetente, recebedor, valor, status ) values ({transacao.Id}, {transacao.Remetente}, {transacao.Recebedor}, {transacao.Valor}, {transacao.Status})";
        var connection = DbConnection();
        SQLiteCommand cmd = new(sql, connection);
        cmd.ExecuteNonQuery();
        connection.Close();
      }
      catch
      {
        throw;
      }
    }

    public static void AddValidador(Validador validador)
    {
      try
      {
        var connection = DbConnection();
        SQLiteCommand cmd = new(connection);

        cmd.CommandText = "INSERT INTO Validadores (id, nome, ip ) values (@id, @nome, @ip)";
        cmd.Parameters.AddWithValue("@Id", validador.Id);
        cmd.Parameters.AddWithValue("@Nome", validador.Nome);
        cmd.Parameters.AddWithValue("@Ip", validador.Ip);

        cmd.ExecuteNonQuery();
        connection.Close();
      }
      catch
      {
        throw;
      }
    }

    public static void DeleteValidador(int id)
    {
      try
      {
        var connection = DbConnection();
        SQLiteCommand cmd = new(connection);

        cmd.CommandText = "DELETE FROM Validadores Where Id=@id";
        cmd.Parameters.AddWithValue("@Id", id);

        cmd.ExecuteNonQuery();
        connection.Close();
      }
      catch
      {
        throw;
      }
    }

    public static void AddSyncTime(int time)
    {
      try
      {
        using var cmd = DbConnection().CreateCommand();
        cmd.CommandText = "INSERT INTO Horarios(time ) values (@time)";
        cmd.Parameters.AddWithValue("@Time", time);
        cmd.ExecuteNonQuery();
      }
      catch
      {
        throw;
      }
    }
  }
}
