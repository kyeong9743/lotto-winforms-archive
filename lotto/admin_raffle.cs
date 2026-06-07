using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace lotto
{
    public partial class admin_raffle : Form
    {
        string school_name = "컴퓨터공학과";
        private Random random = new Random();
        private int currentTextBoxIndex = 1;
        private System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
        private List<int> generatedNumbers = new List<int>();

        public admin_raffle()
        {
            InitializeComponent();
        }

        private void lotto_auto_Click(object sender, EventArgs e)
        {
            StartGeneratingNumbers();
        }

        private void admin_raffle_Load(object sender, EventArgs e)
        {
            datagridviewload();
            timer.Interval = 1000;
            timer.Tick += Timer_Tick;
        }

        int[] winNums = new int[6];
        private void StartGeneratingNumbers()
        {
            place_1nd.Text = "";
            place_2nd.Text = "";
            place_3nd.Text = "";

            if (currentTextBoxIndex <= 6)
            {
                int randomNumber;
                do { randomNumber = random.Next(1, 46); } while (generatedNumbers.Contains(randomNumber));

                generatedNumbers.Add(randomNumber);
                winNums[currentTextBoxIndex - 1] = randomNumber;

                string buttonName = "win_" + currentTextBoxIndex;
                var controls = this.Controls.Find(buttonName, true);
                if (controls.Length > 0 && controls[0] is Bunifu.UI.WinForms.BunifuButton.BunifuButton button)
                {
                    button.Text = randomNumber.ToString();
                }

                currentTextBoxIndex++;
                if (currentTextBoxIndex <= 7) timer.Start();
            }
            else
            {
                raffle();
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            timer.Stop();
            StartGeneratingNumbers();
        }

        private void admin_makedata_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("[테스트번호추가]실행하시겠습니까?\r\n이 작업은 되돌릴 수 없습니다.", school_name, MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                try
                {
                    using (SQLiteConnection connection = new SQLiteConnection(SharedUtils.ConnectionString))
                    {
                        connection.Open();
                        using (var transaction = connection.BeginTransaction())
                        {
                            string insertQuery = "INSERT INTO lottodata (idnumber, num1, num2, num3, num4, num5, timetable) VALUES (@idnumber, @num1, @num2, @num3, @num4, @num5, @timetable)";
                            using (SQLiteCommand insertCommand = new SQLiteCommand(insertQuery, connection))
                            {
                                for (int i = 0; i < 3000; i++)
                                {
                                    List<int> used = new List<int>();
                                    for (int j = 1; j <= 5; j++)
                                    {
                                        int n;
                                        do { n = random.Next(1, 46); } while (used.Contains(n));
                                        used.Add(n);
                                        insertCommand.Parameters.AddWithValue("@num" + j, n);
                                    }

                                    insertCommand.Parameters.AddWithValue("@idnumber", AdminGenerateRandomUserId());
                                    insertCommand.Parameters.AddWithValue("@timetable", DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                                    insertCommand.ExecuteNonQuery();
                                    insertCommand.Parameters.Clear();
                                }
                            }
                            transaction.Commit();
                        }
                    }
                    MessageBox.Show("작업완료", school_name);
                    datagridviewload();
                }
                catch (Exception ex) { MessageBox.Show($"{ex}", school_name); }
            }
        }

        private string AdminGenerateRandomUserId()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, 8).Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private void datagridviewload()
        {
            try
            {
                using (SQLiteConnection con = new SQLiteConnection(SharedUtils.ConnectionString))
                {
                    con.Open();
                    string strSql = "SELECT * FROM lottodata";
                    SQLiteDataAdapter da = new SQLiteDataAdapter(strSql, con);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    dt.Columns.Add("장 수", typeof(int));
                    for (int i = 0; i < dt.Rows.Count; i++) dt.Rows[i]["장 수"] = i + 1;

                    DataGridView1.DataSource = dt;
                    DataGridView1.Columns["id"].Visible = false;
                    DataGridView1.Columns["idnumber"].Visible = false;
                    DataGridView1.Columns["장 수"].DisplayIndex = 0;
                    DataGridView1.Columns["timetable"].HeaderText = "구매시간";
                    DataGridView1.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }
            }
            catch (Exception ex) { MessageBox.Show($"{ex}", school_name); }
        }

        private void raffle()
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(SharedUtils.ConnectionString))
                {
                    connection.Open();
                    string selectQuery = "SELECT * FROM lottodata";
                    StringBuilder w1 = new StringBuilder(), w2 = new StringBuilder(), w3 = new StringBuilder();

                    using (SQLiteCommand cmd = new SQLiteCommand(selectQuery, connection))
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int[] nums = { reader.GetInt32(2), reader.GetInt32(3), reader.GetInt32(4), reader.GetInt32(5), reader.GetInt32(6) };
                            string user = reader.GetString(1);
                            string time = reader.GetString(7);
                            int matchCount = nums.Count(n => winNums.Contains(n));

                            if (matchCount == 5 && nums.All(n => winNums.Take(5).Contains(n)))
                            {
                                w1.AppendLine($"{user}\r\n{string.Join(", ", nums)}\r\n\r\n");
                                SharedUtils.SendWebhook("로또 당첨자 리스트", user, string.Join(", ", nums), time, "등 수 : 1등");
                            }
                            else if (matchCount == 5)
                            {
                                w2.AppendLine($"{user}\r\n{string.Join(", ", nums)}\r\n\r\n");
                                SharedUtils.SendWebhook("로또 당첨자 리스트", user, string.Join(", ", nums), time, "등 수 : 2등");
                            }
                            else if (matchCount == 4)
                            {
                                w3.AppendLine($"{user}\r\n{string.Join(", ", nums)}\r\n\r\n");
                                SharedUtils.SendWebhook("로또 당첨자 리스트", user, string.Join(", ", nums), time, "등 수 : 3등");
                            }
                        }
                    }

                    place_1nd.Text = w1.Length > 0 ? w1.ToString() : "1등 당첨자 없음";
                    place_2nd.Text = w2.Length > 0 ? w2.ToString() : "2등 당첨자 없음";
                    place_3nd.Text = w3.Length > 0 ? w3.ToString() : "3등 당첨자 없음";
                }
            }
            catch (Exception e) { MessageBox.Show(e.Message); }
        }
    }
}
