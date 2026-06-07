using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;

namespace lotto
{
    public partial class lotto_input_form : Form
    {
        public lotto_input_form()
        {
            InitializeComponent();
            this.KeyPreview = true;
        }
        private bool isFullScreen = false;

        string school_name = "컴퓨터공학과";
        private char[] alphabets = { 'A', 'B', 'C', 'D', 'E' };
        private int currentAlphabetIndex = 0;

        private void confirm_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(student_number.Text))
            {
                MessageBox.Show("학번 또는 전화번호를 입력해주세요!", school_name);
            }
            else
            {
                webView21.Visible = true;

                string student_number_info = student_number.Text;

                CheckAndInsert(student_number_info, a_1.Text, a_2.Text, a_3.Text, a_4.Text, a_5.Text);
                CheckAndInsert(student_number_info, b_1.Text, b_2.Text, b_3.Text, b_4.Text, b_5.Text);
                CheckAndInsert(student_number_info, c_1.Text, c_2.Text, c_3.Text, c_4.Text, c_5.Text);
                CheckAndInsert(student_number_info, d_1.Text, d_2.Text, d_3.Text, d_4.Text, d_5.Text);
                CheckAndInsert(student_number_info, e_1.Text, e_2.Text, e_3.Text, e_4.Text, e_5.Text);

                webView21.Visible = false;
                MessageBox.Show("구매가 완료되었습니다!", school_name);

                ClearInputs();
                reset_button();
                lotto_number.Text = alphabets[0].ToString();
                currentAlphabetIndex = 0;
            }
        }

        private void CheckAndInsert(string student_number_info, string n1, string n2, string n3, string n4, string n5)
        {
            if (!string.IsNullOrEmpty(n1) || !string.IsNullOrEmpty(n2) || !string.IsNullOrEmpty(n3) || !string.IsNullOrEmpty(n4) || !string.IsNullOrEmpty(n5))
            {
                InsertData(student_number_info, n1, n2, n3, n4, n5);
            }
        }

        private void ClearInputs()
        {
            Control[] controls = { a_1, a_2, a_3, a_4, a_5, b_1, b_2, b_3, b_4, b_5, c_1, c_2, c_3, c_4, c_5, d_1, d_2, d_3, d_4, d_5, e_1, e_2, e_3, e_4, e_5 };
            foreach (var c in controls) c.Text = "";
        }

        private void InsertData(string student_number_info, string num1, string num2, string num3, string num4, string num5)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(SharedUtils.ConnectionString))
                {
                    connection.Open();
                    string insertQuery = "INSERT INTO lottodata (idnumber, num1, num2, num3, num4, num5, timetable) VALUES (@idnumber, @num1, @num2, @num3, @num4, @num5, @timetable)";

                    using (SQLiteCommand insertCommand = new SQLiteCommand(insertQuery, connection))
                    {
                        insertCommand.Parameters.AddWithValue("@idnumber", student_number_info);
                        insertCommand.Parameters.AddWithValue("@num1", num1);
                        insertCommand.Parameters.AddWithValue("@num2", num2);
                        insertCommand.Parameters.AddWithValue("@num3", num3);
                        insertCommand.Parameters.AddWithValue("@num4", num4);
                        insertCommand.Parameters.AddWithValue("@num5", num5);
                        insertCommand.Parameters.AddWithValue("@timetable", DateTime.Now.ToString("yyyy-MM-dd HH:mm"));

                        if (insertCommand.ExecuteNonQuery() != 1)
                        {
                            MessageBox.Show("오류가 발생했습니다.[1]");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"오류 : {ex}");
            }

            SharedUtils.SendWebhook("로또 입력 기록", student_number_info, $"{num1}, {num2}, {num3}, {num4}, {num5}", DateTime.Now.ToString("yyyy년 MM월 dd일 HH시 mm분"), "Pos번호: 1번");
        }

        private void lotto_input_form_Load(object sender, EventArgs e)
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
            isFullScreen = true;

            webView21.Anchor = AnchorStyles.None;
            webView21.Left = (this.ClientSize.Width - webView21.Width) / 2;
            webView21.Top = (this.ClientSize.Height - webView21.Height) / 2;

            string htmlFilePath = Path.Combine(Environment.CurrentDirectory, "loading.html");
            webView21.Source = new Uri("file:///" + htmlFilePath);
            webView21.Visible = false;
        }

        private void lotto_list(int number)
        {
            string buttonName = "lotto_num_" + number;
            Control[] foundControls = this.Controls.Find(buttonName, true);

            if (foundControls.Length > 0 && foundControls[0] is Bunifu.UI.WinForms.BunifuButton.BunifuButton button)
            {
                if (button.Text == "✔") return;
            }

            char currentAlphabet = alphabets[currentAlphabetIndex];
            Bunifu.UI.WinForms.BunifuButton.BunifuButton[] buttons = null;

            switch (currentAlphabet)
            {
                case 'A': buttons = new[] { a_1, a_2, a_3, a_4, a_5 }; break;
                case 'B': buttons = new[] { b_1, b_2, b_3, b_4, b_5 }; break;
                case 'C': buttons = new[] { c_1, c_2, c_3, c_4, c_5 }; break;
                case 'D': buttons = new[] { d_1, d_2, d_3, d_4, d_5 }; break;
                case 'E': buttons = new[] { e_1, e_2, e_3, e_4, e_5 }; break;
            }

            if (buttons != null)
            {
                bool isDuplicate = buttons.Any(btn => btn.Text == number.ToString());
                bool isFull = buttons.All(btn => !string.IsNullOrEmpty(btn.Text));

                if (isDuplicate)
                {
                    MessageBox.Show("중복 입력입니다.", school_name);
                }
                else if (!isFull)
                {
                    foreach (var btn in buttons)
                    {
                        if (string.IsNullOrEmpty(btn.Text))
                        {
                            btn.Text = number.ToString();
                            break;
                        }
                    }

                    if (foundControls.Length > 0 && foundControls[0] is Bunifu.UI.WinForms.BunifuButton.BunifuButton targetButton)
                    {
                        targetButton.Text = "✔";
                    }
                }
            }
        }

        private void nextbtn_Click(object sender, EventArgs e)
        {
            currentAlphabetIndex = (currentAlphabetIndex + 1) % alphabets.Length;
            lotto_number.Text = alphabets[currentAlphabetIndex].ToString();
            reset_button();
        }

        private void previousbtn_Click(object sender, EventArgs e)
        {
            currentAlphabetIndex = (currentAlphabetIndex - 1 + alphabets.Length) % alphabets.Length;
            lotto_number.Text = alphabets[currentAlphabetIndex].ToString();
            reset_button();
        }

        private void lotto_num_1_Click(object sender, EventArgs e) => lotto_list(1);
        private void lotto_num_2_Click(object sender, EventArgs e) => lotto_list(2);
        private void lotto_num_3_Click(object sender, EventArgs e) => lotto_list(3);
        private void lotto_num_4_Click(object sender, EventArgs e) => lotto_list(4);
        private void lotto_num_5_Click(object sender, EventArgs e) => lotto_list(5);
        private void lotto_num_6_Click(object sender, EventArgs e) => lotto_list(6);
        private void lotto_num_7_Click(object sender, EventArgs e) => lotto_list(7);
        private void lotto_num_8_Click(object sender, EventArgs e) => lotto_list(8);
        private void lotto_num_9_Click(object sender, EventArgs e) => lotto_list(9);
        private void lotto_num_10_Click(object sender, EventArgs e) => lotto_list(10);
        private void lotto_num_11_Click(object sender, EventArgs e) => lotto_list(11);
        private void lotto_num_12_Click(object sender, EventArgs e) => lotto_list(12);
        private void lotto_num_13_Click(object sender, EventArgs e) => lotto_list(13);
        private void lotto_num_14_Click(object sender, EventArgs e) => lotto_list(14);
        private void lotto_num_15_Click(object sender, EventArgs e) => lotto_list(15);
        private void lotto_num_16_Click(object sender, EventArgs e) => lotto_list(16);
        private void lotto_num_17_Click(object sender, EventArgs e) => lotto_list(17);
        private void lotto_num_18_Click(object sender, EventArgs e) => lotto_list(18);
        private void lotto_num_19_Click(object sender, EventArgs e) => lotto_list(19);
        private void lotto_num_20_Click(object sender, EventArgs e) => lotto_list(20);
        private void lotto_num_21_Click(object sender, EventArgs e) => lotto_list(21);
        private void lotto_num_22_Click(object sender, EventArgs e) => lotto_list(22);
        private void lotto_num_23_Click(object sender, EventArgs e) => lotto_list(23);
        private void lotto_num_24_Click(object sender, EventArgs e) => lotto_list(24);
        private void lotto_num_25_Click(object sender, EventArgs e) => lotto_list(25);
        private void lotto_num_26_Click(object sender, EventArgs e) => lotto_list(26);
        private void lotto_num_27_Click(object sender, EventArgs e) => lotto_list(27);
        private void lotto_num_28_Click(object sender, EventArgs e) => lotto_list(28);
        private void lotto_num_29_Click(object sender, EventArgs e) => lotto_list(29);
        private void lotto_num_30_Click(object sender, EventArgs e) => lotto_list(30);
        private void lotto_num_31_Click(object sender, EventArgs e) => lotto_list(31);
        private void lotto_num_32_Click(object sender, EventArgs e) => lotto_list(32);
        private void lotto_num_33_Click(object sender, EventArgs e) => lotto_list(33);
        private void lotto_num_34_Click(object sender, EventArgs e) => lotto_list(34);
        private void lotto_num_35_Click(object sender, EventArgs e) => lotto_list(35);
        private void lotto_num_36_Click(object sender, EventArgs e) => lotto_list(36);
        private void lotto_num_37_Click(object sender, EventArgs e) => lotto_list(37);
        private void lotto_num_38_Click(object sender, EventArgs e) => lotto_list(38);
        private void lotto_num_39_Click(object sender, EventArgs e) => lotto_list(39);
        private void lotto_num_40_Click(object sender, EventArgs e) => lotto_list(40);
        private void lotto_num_41_Click(object sender, EventArgs e) => lotto_list(41);
        private void lotto_num_42_Click(object sender, EventArgs e) => lotto_list(42);
        private void lotto_num_43_Click(object sender, EventArgs e) => lotto_list(43);
        private void lotto_num_44_Click(object sender, EventArgs e) => lotto_list(44);
        private void lotto_num_45_Click(object sender, EventArgs e) => lotto_list(45);

        private void lotto_auto_Click(object sender, EventArgs e)
        {
            Random random = new Random();
            List<int> numbers = Enumerable.Range(1, 45).ToList();
            List<int> selected = new List<int>();

            for (int i = 0; i < 5; i++)
            {
                int index = random.Next(numbers.Count);
                selected.Add(numbers[index]);
                numbers.RemoveAt(index);
            }

            foreach (var n in selected) lotto_list(n);
        }

        private void a_1_Click(object sender, EventArgs e) { reset_early_button(a_1.Text); a_1.Text = ""; }
        private void a_2_Click(object sender, EventArgs e) { reset_early_button(a_2.Text); a_2.Text = ""; }
        private void a_3_Click(object sender, EventArgs e) { reset_early_button(a_3.Text); a_3.Text = ""; }
        private void a_4_Click(object sender, EventArgs e) { reset_early_button(a_4.Text); a_4.Text = ""; }
        private void a_5_Click(object sender, EventArgs e) { reset_early_button(a_5.Text); a_5.Text = ""; }
        private void b_1_Click(object sender, EventArgs e) { reset_early_button(b_1.Text); b_1.Text = ""; }
        private void b_2_Click(object sender, EventArgs e) { reset_early_button(b_2.Text); b_2.Text = ""; }
        private void b_3_Click(object sender, EventArgs e) { reset_early_button(b_3.Text); b_3.Text = ""; }
        private void b_4_Click(object sender, EventArgs e) { reset_early_button(b_4.Text); b_4.Text = ""; }
        private void b_5_Click(object sender, EventArgs e) { reset_early_button(b_5.Text); b_5.Text = ""; }
        private void c_1_Click(object sender, EventArgs e) { reset_early_button(c_1.Text); c_1.Text = ""; }
        private void c_2_Click(object sender, EventArgs e) { reset_early_button(c_2.Text); c_2.Text = ""; }
        private void c_3_Click(object sender, EventArgs e) { reset_early_button(c_3.Text); c_3.Text = ""; }
        private void c_4_Click(object sender, EventArgs e) { reset_early_button(c_4.Text); c_4.Text = ""; }
        private void c_5_Click(object sender, EventArgs e) { reset_early_button(c_5.Text); c_5.Text = ""; }
        private void d_1_Click(object sender, EventArgs e) { reset_early_button(d_1.Text); d_1.Text = ""; }
        private void d_2_Click(object sender, EventArgs e) { reset_early_button(d_2.Text); d_2.Text = ""; }
        private void d_3_Click(object sender, EventArgs e) { reset_early_button(d_3.Text); d_3.Text = ""; }
        private void d_4_Click(object sender, EventArgs e) { reset_early_button(d_4.Text); d_4.Text = ""; }
        private void d_5_Click(object sender, EventArgs e) { reset_early_button(d_5.Text); d_5.Text = ""; }
        private void e_1_Click(object sender, EventArgs e) { reset_early_button(e_1.Text); e_1.Text = ""; }
        private void e_2_Click(object sender, EventArgs e) { reset_early_button(e_2.Text); e_2.Text = ""; }
        private void e_3_Click(object sender, EventArgs e) { reset_early_button(e_3.Text); e_3.Text = ""; }
        private void e_4_Click(object sender, EventArgs e) { reset_early_button(e_4.Text); e_4.Text = ""; }
        private void e_5_Click(object sender, EventArgs e) { reset_early_button(e_5.Text); e_5.Text = ""; }

        private void reset_a_Click(object sender, EventArgs e) { a_1.Text = a_2.Text = a_3.Text = a_4.Text = a_5.Text = ""; reset_button(); }
        private void reset_b_Click(object sender, EventArgs e) { b_1.Text = b_2.Text = b_3.Text = b_4.Text = b_5.Text = ""; reset_button(); }
        private void reset_c_Click(object sender, EventArgs e) { c_1.Text = c_2.Text = c_3.Text = c_4.Text = c_5.Text = ""; reset_button(); }
        private void reset_d_Click(object sender, EventArgs e) { d_1.Text = d_2.Text = d_3.Text = d_4.Text = d_5.Text = ""; reset_button(); }
        private void reset_e_Click(object sender, EventArgs e) { e_1.Text = e_2.Text = e_3.Text = e_4.Text = e_5.Text = ""; reset_button(); }

        private void reset_button()
        {
            for (int i = 1; i <= 45; i++)
            {
                string buttonName = "lotto_num_" + i;
                Control[] foundControls = this.Controls.Find(buttonName, true);
                if (foundControls.Length > 0 && foundControls[0] is Bunifu.UI.WinForms.BunifuButton.BunifuButton button)
                {
                    button.Text = i.ToString();
                }
            }
            student_number.Text = "";
        }

        private void reset_early_button(string number)
        {
            if (string.IsNullOrEmpty(number)) return;
            string buttonName = "lotto_num_" + number;
            Control[] foundControls = this.Controls.Find(buttonName, true);
            if (foundControls.Length > 0 && foundControls[0] is Bunifu.UI.WinForms.BunifuButton.BunifuButton button)
            {
                button.Text = number;
            }
        }

        private void lotto_input_form_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.Shift && e.KeyCode == Keys.F4)
            {
                this.FormBorderStyle = isFullScreen ? FormBorderStyle.Sizable : FormBorderStyle.None;
                this.WindowState = isFullScreen ? FormWindowState.Normal : FormWindowState.Maximized;
                isFullScreen = !isFullScreen;
                e.Handled = true;
            }

            if (e.Control && e.Shift && e.KeyCode == Keys.F5)
            {
                lotto_input_form lotto_input_form = new lotto_input_form();
                this.Hide();
                lotto_input_form.ShowDialog();
                this.Close();
                e.Handled = true;
            }
        }
    }
}
