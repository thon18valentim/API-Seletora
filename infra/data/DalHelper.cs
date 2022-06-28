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
        string sql = $"CREATE TABLE IF NOT EXISTS Transacoes (id int, remetente int, recebedor int, valor int, horario Varchar(20), status int)";
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
        string sql = $"CREATE TABLE IF NOT EXISTS Validadores (id int, nome Varchar(20), ip Varchar(20), stake int)";
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

    public static void CriarTabelaChaveUnicaValidadoresSQlite()
    {
      try
      {
        string sql = $"CREATE TABLE IF NOT EXISTS ChaveUnica (id int, chave Varchar(20))";
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

    public static void CriarTabelaTempoSQlite()
    {
      try
      {
        string sql = $"CREATE TABLE IF NOT EXISTS Tempo (time Varchar(20))";
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

    public static void CriarTabelaPunicoes()
    {
      try
      {
        string sql = $"CREATE TABLE IF NOT EXISTS Punicoes (id int, erros int, ativa int, inicio Varchar(20))";
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
            Horario = dr.GetString(4),
            Status = dr.GetInt32(5)
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
            Ip = dr.GetString(2),
            Stake = dr.GetInt32(3)
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

    public static List<string> GetChaveUnica()
    {
      try
      {
        string sql = $"SELECT * FROM ChaveUnica";
        var connection = DbConnection();
        SQLiteCommand cmd = new(sql, connection);
        SQLiteDataReader dr = cmd.ExecuteReader();

        List<string> chaves = new();
        while (dr.Read())
        {
          chaves.Add(dr.GetString(1));
        }
        connection.Close();

        return chaves;
      }
      catch
      {
        throw;
      }
    }

    public static string GetUmaChaveUnica(int id)
    {
      try
      {
        string sql = $"SELECT * FROM ChaveUnica";
        var connection = DbConnection();
        SQLiteCommand cmd = new(sql, connection);
        SQLiteDataReader dr = cmd.ExecuteReader();

        var chave = "";
        while (dr.Read())
        {
          if(dr.GetInt32(0) == id)
          {
            chave = dr.GetString(1);
          }
        }
        connection.Close();

        return chave;
      }
      catch
      {
        throw;
      }
    }

    public static string GetApiTime()
    {
      try
      {
        string sql = $"SELECT * FROM Tempo";
        var connection = DbConnection();
        SQLiteCommand cmd = new(sql, connection);
        SQLiteDataReader dr = cmd.ExecuteReader();

        List<string> list = new();
        while (dr.Read())
        {
          var tempo = dr.GetString(0);
          list.Add(tempo);
        }
        connection.Close();
        list.Reverse();
        
        if(list.Count == 0)
        {
          return "";
        }
        return list[0];
      }
      catch
      {
        throw;
      }
    }

    public static Transacao GetTransacaoById(int id)
    {
      try
      {
        var connection = DbConnection();
        SQLiteCommand cmd = new(connection);

        cmd.CommandText = $"SELECT * FROM Transacoes Where Id=@id";
        cmd.Parameters.AddWithValue("@Id", id);
        SQLiteDataReader dr = cmd.ExecuteReader();

        var transacao = new Transacao();
        while (dr.Read())
        {
          transacao.Id = dr.GetInt32(0);
          transacao.Remetente = dr.GetInt32(1);
          transacao.Recebedor = dr.GetInt32(2);
          transacao.Valor = dr.GetInt32(3);
          transacao.Horario = dr.GetString(4);
          transacao.Status = dr.GetInt32(5);
        }
        connection.Close();
        return transacao;
      }
      catch
      {
        throw;
      }
    }

    public static Punicao VerificarPunicao(int id)
    {
      try
      {
        var connection = DbConnection();
        SQLiteCommand cmd = new(connection);

        cmd.CommandText = $"SELECT * FROM Punicoes Where Id=@id";
        cmd.Parameters.AddWithValue("@Id", id);
        SQLiteDataReader dr = cmd.ExecuteReader();

        var punicao = new Punicao();
        while (dr.Read())
        {
          punicao.Id = dr.GetInt32(0);
          punicao.Erros = dr.GetInt32(1);
          punicao.Ativa = dr.GetInt32(2);
          punicao.Inicio = dr.GetString(3);
        }

        connection.Close();
        return punicao;
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
        var connection = DbConnection();
        SQLiteCommand cmd = new(connection);

        cmd.CommandText = $"INSERT INTO Transacoes (id, remetente, recebedor, valor, horario, status ) values (@id, @remetente, @recebedor, @valor, @horario, @status)";
        cmd.Parameters.AddWithValue("@Id", transacao.Id);
        cmd.Parameters.AddWithValue("@Remetente", transacao.Remetente);
        cmd.Parameters.AddWithValue("@Recebedor", transacao.Recebedor);
        cmd.Parameters.AddWithValue("@Valor", transacao.Valor);
        cmd.Parameters.AddWithValue("@Horario", transacao.Horario);
        cmd.Parameters.AddWithValue("@Status", transacao.Status);

        cmd.ExecuteNonQuery();
        connection.Close();
      }
      catch
      {
        throw;
      }
    }

    public static void AddTempo(string tempo)
    {
      try
      {
        var connection = DbConnection();
        SQLiteCommand cmd = new(connection);

        cmd.CommandText = "INSERT INTO Tempo (tempo ) values (@tempo)";
        cmd.Parameters.AddWithValue("@Tempo", tempo);
        
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

        cmd.CommandText = "INSERT INTO Validadores (id, nome, ip, stake ) values (@id, @nome, @ip, @stake)";
        cmd.Parameters.AddWithValue("@Id", validador.Id);
        cmd.Parameters.AddWithValue("@Nome", validador.Nome);
        cmd.Parameters.AddWithValue("@Ip", validador.Ip);
        cmd.Parameters.AddWithValue("@stake", validador.Stake);

        cmd.ExecuteNonQuery();
        connection.Close();
      }
      catch
      {
        throw;
      }
    }

    public static void AddChaveUnica(int id, string chave)
    {
      try
      {
        var connection = DbConnection();
        SQLiteCommand cmd = new(connection);

        cmd.CommandText = "INSERT INTO ChaveUnica (id, chave) values (@id, @chave)";
        cmd.Parameters.AddWithValue("@Id", id);
        cmd.Parameters.AddWithValue("@Chave", chave);

        cmd.ExecuteNonQuery();
        connection.Close();
      }
      catch
      {
        throw;
      }
    }

    public static void AddPunicao(int id)
    {
      try
      {
        var connection = DbConnection();
        SQLiteCommand cmd = new(connection);

        cmd.CommandText = "INSERT INTO Punicoes (id, erros, ativa, inicio) values (@id, @erros, @ativa, @inicio)";
        cmd.Parameters.AddWithValue("@Id", id);
        cmd.Parameters.AddWithValue("@Erros", 1);
        cmd.Parameters.AddWithValue("@Ativa", 0);
        cmd.Parameters.AddWithValue("@Inicio", "0");

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

    public static void UpdateValidador(Validador validador)
    {
      try
      {
        var connection = DbConnection();
        SQLiteCommand cmd = new(connection);

        cmd.CommandText = "UPDATE Validadores SET Stake=@stake WHERE Id=@Id";
        cmd.Parameters.AddWithValue("@Id", validador.Id);
        cmd.Parameters.AddWithValue("@Stake", validador.Stake);

        cmd.ExecuteNonQuery();
        connection.Close();
      }
      catch
      {
        throw;
      }
    }

    public static void UpdatePunicao(int id, int erros, int ativa)
    {
      try
      {
        var connection = DbConnection();
        SQLiteCommand cmd = new(connection);

        cmd.CommandText = "UPDATE Punicoes SET Erros=@erros, Ativa=@ativa, Inicio=@inicio WHERE Id=@Id";
        cmd.Parameters.AddWithValue("@Id", id);
        cmd.Parameters.AddWithValue("@Erros", erros);
        cmd.Parameters.AddWithValue("@Ativa", ativa);

        if(ativa == 1)
        {
          DateTime foo = DateTime.Now;
          var horario = ((DateTimeOffset)foo).ToUnixTimeSeconds().ToString();
          cmd.Parameters.AddWithValue("@Inicio", horario);
        }
        else
        {
          cmd.Parameters.AddWithValue("@Inicio", "0");
        }

        cmd.ExecuteNonQuery();
        connection.Close();
      }
      catch
      {
        throw;
      }
    }
  }
}
