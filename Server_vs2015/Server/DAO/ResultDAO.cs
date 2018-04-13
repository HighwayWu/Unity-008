using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameServer.Model;
using MySql.Data.MySqlClient;

namespace GameServer.DAO
{
    class ResultDAO
    {
        public Result GetResultByUserID(MySqlConnection conn, int userID)
        {
            MySqlDataReader reader = null;
            try
            {
                MySqlCommand cmd = new MySqlCommand("select * from result where userid=@userid", conn);
                cmd.Parameters.AddWithValue("userid", userID);
                reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    int id = reader.GetInt32("id");
                    int winCount = reader.GetInt32("wincount");
                    int totalCount = reader.GetInt32("totalcount");
                    return new Result(id, userID, totalCount, winCount);
                }
                else
                {
                    return new Result(-1, userID, 0, 0);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("在获取历史记录时出现异常: " + e);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
            return null;
        }

        public void UpdateOrAddResult(MySqlConnection conn, Result result)
        {
            try
            {
                MySqlCommand cmd = null;
                if(result.ID <= -1)
                {
                    cmd = new MySqlCommand("insert into result set totalcount=@totalcount, wincount=@wincount, userid=@userid", conn);
                }
                else
                {
                    cmd = new MySqlCommand("update result set totalcount=@totalcount, wincount=@wincount where userid=@userid", conn);
                }
                cmd.Parameters.AddWithValue("totalcount", result.TotalCount);
                cmd.Parameters.AddWithValue("wincount", result.WinCount);
                cmd.Parameters.AddWithValue("userid", result.UserID);
                cmd.ExecuteNonQuery();
                if(result.ID <= -1)
                {
                    Result tempRes = GetResultByUserID(conn, result.UserID);
                    result.ID = tempRes.ID;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("在更新记录时出现异常: " + e);
            }
        }
    }
}
