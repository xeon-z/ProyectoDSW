using Microsoft.Data.SqlClient;


namespace ProyectoDSW_Cosmeticos.DAO
{
    public class conexionDAO
    {

        SqlConnection cn = new SqlConnection(@"server = LAPTOP-7LMDPBUB\MSSQLSERVER01;database = Cosmetica;Trusted_Connection = False;uid = sa;pwd = sql;MultipleActiveResultSets = True;TrustServerCertificate = False;Encrypt = False");

        public SqlConnection getcn
        {
            get { return cn; }
        }

    }
}
