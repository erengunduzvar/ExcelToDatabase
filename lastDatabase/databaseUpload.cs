using ExcelDataReader;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace lastDatabase
{
    internal class databaseUpload
    {

        private readonly Timer _timer;
        MySqlConnection con;
        List<int> rowIds = new List<int>();
        public databaseUpload()
        {

            _timer = new Timer(Assets.tickTimer) { AutoReset = true };
            _timer.Elapsed += _timer_Elapsed;

        }

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {

            string[] lines = new string[] { DateTime.Now.ToString() };
            if (!File.Exists(Assets.ExcelFilePath))
            {
                Console.WriteLine("Excel Dosyasi yok!");
            }
            else
            {
                try
                {
                    File.AppendAllLines(Assets.LogFilePath, lines);
                    Console.WriteLine("trying");

                    con = new MySqlConnection(Assets.ConnectionString);
                    con.Open();
                    Console.WriteLine(con.State);

                    try
                    {
                        using (var stream = File.Open(Assets.ExcelFilePath, FileMode.Open, FileAccess.Read))
                        {
                            using (var reader = ExcelReaderFactory.CreateReader(stream))
                            {
                                int count = 0;
                                do
                                {
                                    while (reader.Read())
                                    {
                                        if (count != 0)
                                        {
                                            int y = (int)reader.GetDouble(0);
                                            Console.Write(" - " + reader.GetString(1));
                                            Console.WriteLine(" - " + reader.GetString(2));

                                            Console.WriteLine("id: {0}|name: {1}|surname:{2}", (int)reader.GetDouble(0), reader.GetString(1), reader.GetString(2));
                                            //statik dinamik olacak
                                            int idUser = (int)reader.GetDouble(0),
                                            idRepresentetive = (int)reader.GetDouble(7);
                                            string adressUser = reader.GetString(5),
                                                emailUser = reader.GetString(4),
                                                mobileNumUser = reader.GetValue(3).ToString(),
                                                nameUser = reader.GetString(1),
                                                surnameUser = reader.GetString(2),
                                                tcNoUser = reader.GetValue(6).ToString();

                                            if (nameUser.Length == 0)
                                                break;

                                            try
                                            {
                                                string sql = "INSERT INTO `projectdatabase`.`user` (`idUser`, `nameUser`, `surnameUser`, `mobileNumUser`, `emailUser`, `addressUser`, `tcNoUser`, `idRepresentative`) VALUES ('" + idUser + "', '" + nameUser + "', '" + surnameUser + "', '" + mobileNumUser + "', '" + emailUser + "', '" + adressUser + "', '" + tcNoUser + "', '" + idRepresentetive + "')";
                                                MySqlCommand cmd = new MySqlCommand(sql, con);
                                                File.AppendAllText(@"C:\Users\eymen\Desktop\Mikroservis test\Log.txt", "id :" + idUser + Environment.NewLine);
                                                cmd.ExecuteNonQuery();
                                                Console.WriteLine("Create a new input");
                                                rowIds.Add(idUser);
                                            }
                                            catch (Exception a)
                                            {
                                                //  Console.WriteLine(a);
                                                if (a.Message.Contains("Duplicate entry") && a.Message.Contains("user.PRIMARY"))
                                                {
                                                    try
                                                    {
                                                        Console.WriteLine("This entry already exists\nUpdating old data...");
                                                        string sql = "UPDATE `projectdatabase`.`user` SET `nameUser` = '" + nameUser + "', `surnameUser` = '" + surnameUser + "', `mobileNumUser` = '" + mobileNumUser + "', `emailUser` = '" + emailUser + "', `addressUser` = '" + adressUser + "', `idRepresentative` = '" + idRepresentetive + "' WHERE (`idUser` = '" + idUser + "')";
                                                        MySqlCommand cmd = new MySqlCommand(sql, con);
                                                        cmd.ExecuteNonQuery();
                                                        rowIds.Add(idUser);
                                                    }
                                                    catch (Exception b)
                                                    {
                                                        Console.WriteLine(b);
                                                    }
                                                }
                                                else if (a.Message.Contains("Duplicate entry") && a.Message.Contains("user.tcNoUser_UNIQUE"))
                                                {
                                                    Console.WriteLine("Different entries cannot take same Tc No");
                                                }
                                            }
                                        }

                                        count++;
                                    }
                                } while (reader.NextResult());
                            }
                        }
                    }
                    catch (Exception a)
                    {
                        if (a.Message.Contains("İşlem, başka bir işlem tarafından kullanıldığından"))
                        {
                            Console.WriteLine("Excel dosyasi acik !!");
                        }
                        else
                            Console.WriteLine(a);
                    }

                    //try
                    //{
                    //    string sql = "SELECT * FROM user";
                    //    var cmd = new MySqlCommand(sql, con);
                    //    List<int> deleteList = new List<int>();
                    //    MySqlDataReader rdr = cmd.ExecuteReader();

                    //    while (rdr.Read())
                    //    {
                    //        Console.WriteLine("idUser : {0}", rdr.GetInt32(0));
                    //        if (!rowIds.Contains(rdr.GetInt32(0)))
                    //        {
                    //            Console.WriteLine("Deleted : " + rdr.GetInt32(0));
                    //            deleteList.Add(rdr.GetInt32(0));
                    //        }
                    //        else
                    //        {
                    //            Console.WriteLine("Stored : " + rdr.GetInt32(0));
                    //        }
                    //    }
                    //    rdr.Close();

                    //    foreach (int id in deleteList)
                    //    {
                    //        try
                    //        {
                    //            sql = "DELETE FROM `projectdatabase`.`user` WHERE (`idUser` = '" + id + "')";
                    //            cmd = new MySqlCommand(sql, con);
                    //            cmd.ExecuteNonQuery();
                    //        }
                    //        catch (Exception a)
                    //        {
                    //            Console.WriteLine(a);
                    //        }

                    //    }
                    //}
                    //catch (Exception l)
                    //{
                    //    Console.WriteLine(l);
                    //}

                    Console.WriteLine("Test Finished!");
                    File.Delete(Assets.ExcelFilePath);
                }
                catch (Exception t)
                {
                    Console.WriteLine(t);
                }





            }

        }
        public void Start()
        {
            _timer.Start();
        }
        public void Stop()
        {
            _timer.Stop();
        }
    }
}
