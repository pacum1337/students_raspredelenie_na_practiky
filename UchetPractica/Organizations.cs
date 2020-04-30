﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UchetPractica
{
    public partial class Organizations : Form
    {
        public Organizations()
        {
            InitializeComponent();
        }

        bool colOrg = false;
        bool close = true;


        private void OrgLoadData(string sqlLoadData = "SELECT * FROM Organizations WHERE Status=N'1'")
        {
            using (SqlConnection connect = new SqlConnection(Strings.ConStr))
            {
                connect.Open();
                SqlDataAdapter adapter = new SqlDataAdapter(sqlLoadData, connect);
                DataSet ds = new DataSet();
                adapter.Fill(ds);
                dataGridView1.DataSource = ds.Tables[0];

                SqlCommand command = new SqlCommand(sqlLoadData, connect);
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    colOrg = true;
                }
                else
                {
                    colOrg = false;
                }
            }
            dataGridView1.Columns[0].Visible = false;

            dataGridView1.Columns[1].HeaderText = "Название организации";
            dataGridView1.Columns[2].HeaderText = "Адрес";
            dataGridView1.Columns[3].HeaderText = "Дата регистрации";
            dataGridView1.Columns[4].HeaderText = "Директор";
            dataGridView1.Columns[5].HeaderText = "ОГРН";
            dataGridView1.Columns[6].HeaderText = "ИНН";
            dataGridView1.Columns[7].HeaderText = "КПП";
            dataGridView1.Columns[8].HeaderText = "ОКПО";
            dataGridView1.Columns[9].HeaderText = "ОКАТО";
            dataGridView1.Columns[10].HeaderText = "ОКОГУ";
            dataGridView1.Columns[11].HeaderText = "ОКТМО";
            dataGridView1.Columns[12].HeaderText = "Кол-во документов с этой организацией";
            dataGridView1.Columns[13].Visible = false;
            dataGridView1.Columns[14].Visible = false;
        }

        private void ProvStudyOrg()
        {
            string sqlSU = "SELECT Id FROM Organizations WHERE StudyOrg = 1";
            using (SqlConnection connection = new SqlConnection(Strings.ConStr))
            {
                connection.Open();
                SqlCommand sql = new SqlCommand(sqlSU, connection);
                SqlDataReader reader = sql.ExecuteReader();
                if (!reader.HasRows)
                {
                    MessageBox.Show("В базе данных нет учебной организации, в которой происходит учет!\n" +
                        "Требуется внести ее в базу данных!");
                    SettingOrganizations setting = new SettingOrganizations();
                    setting.lHeader.Text = "Добавление учебной организации";
                    setting.AddRed = true;
                    setting.cbStatus.Text = "Работающая";
                    setting.checkBox1.Visible = true;
                    setting.checkBox1.Checked = true;
                    setting.ShowDialog();
                    if (!setting.resOrg)
                    {
                        MessageBox.Show("Данные о учебной организации не внесены!\n" +
                            "Надеюсь, что в следущий раз получится)");
                        close = false;
                        Close();
                    }
                }
                OrgLoadData();
            }
        }
        private void выходИзПрограммыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            SettingOrganizations setting = new SettingOrganizations();
            setting.lHeader.Text = "Добавление организации";
            setting.AddRed = true;
            setting.cbStatus.Text = "Работающая";
            setting.ShowDialog();
            OrgLoadData();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (colOrg)
            {
                if (Convert.ToInt32(dataGridView1.CurrentRow.Cells[0].Value) > -1)
                {
                    SettingOrganizations setting = new SettingOrganizations();
                    setting.lHeader.Text = "Редактирование организации";
                    setting.AddRed = false;
                    setting.SelectedOrgId = Convert.ToInt32(dataGridView1.CurrentRow.Cells[0].Value);
                    setting.tbName.Text = Convert.ToString(dataGridView1.CurrentRow.Cells[1].Value);
                    setting.tbAddress.Text = Convert.ToString(dataGridView1.CurrentRow.Cells[2].Value);
                    setting.dtpDateReg.Value = DateTime.Parse(Convert.ToString(dataGridView1.CurrentRow.Cells[3].Value));
                    setting.tbDirector.Text = Convert.ToString(dataGridView1.CurrentRow.Cells[4].Value);
                    setting.tbOGRN.Text = Convert.ToString(dataGridView1.CurrentRow.Cells[5].Value);
                    setting.tbINN.Text = Convert.ToString(dataGridView1.CurrentRow.Cells[6].Value);
                    setting.tbKPP.Text = Convert.ToString(dataGridView1.CurrentRow.Cells[7].Value);
                    setting.tbOKPO.Text = Convert.ToString(dataGridView1.CurrentRow.Cells[8].Value);
                    setting.tbOKATO.Text = Convert.ToString(dataGridView1.CurrentRow.Cells[9].Value);
                    setting.tbOKOGY.Text = Convert.ToString(dataGridView1.CurrentRow.Cells[10].Value);
                    setting.tbOKTMO.Text = Convert.ToString(dataGridView1.CurrentRow.Cells[11].Value);

                    string status = Convert.ToString(dataGridView1.CurrentRow.Cells[13].Value);
                    if (status == "1")
                        setting.cbStatus.Text = "Работающая";
                    else if(status == "2")
                        setting.cbStatus.Text = "Не работающая";
                    setting.ShowDialog();
                    OrgLoadData();
                }
                else
                {
                    MessageBox.Show("Не выбранна запись для редактирования!");
                }
            }
            else
            {
                MessageBox.Show("Нет записей для редактирования!");
            }
        }

        private void Organizations_Load(object sender, EventArgs e)
        {
            ProvStudyOrg();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (colOrg)
            {
                string orgName = Convert.ToString(dataGridView1.CurrentRow.Cells[1].Value);
                if (Convert.ToInt32(dataGridView1.CurrentRow.Cells[0].Value) > 0)
                {
                    DialogResult dr = MessageBox.Show("Вы уверены, что хотите удалить\n" +
                        "организацияю " + orgName + " из базы данных?", "Подтверждение действия",
                        MessageBoxButtons.YesNo);
                    if (dr == DialogResult.Yes)
                    {
                        string sqlDelOrg = String.Format("DELETE FROM Organizations " +
                                    "WHERE Id = '{0}'", Convert.ToInt32(dataGridView1.CurrentRow.Cells[0].Value));


                        using (SqlConnection connection = new SqlConnection(Strings.ConStr))
                        {
                            connection.Open();
                            SqlCommand command = new SqlCommand(sqlDelOrg, connection);
                            int number = command.ExecuteNonQuery();
                            if (number > 0)
                            {
                                MessageBox.Show("Организация " + orgName + " удалена из базы данных");
                                OrgLoadData();
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Выберите организацию для удаления!");
                }
            }
            else
            {
                MessageBox.Show("Нет записей для удаления!");
            }
        }

        private void bCancel_Click(object sender, EventArgs e)
        {
            close = false;
            Close();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            string searchOrg = tbSearch.Text;
            if (searchOrg != "")
            {
                string sqlSearch = String.Format("SELECT * FROM Organizations " +
                    "WHERE Name LIKE N'%{0}%'", searchOrg);

                using (SqlConnection connection = new SqlConnection(Strings.ConStr))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(sqlSearch, connection);
                    DataSet ds = new DataSet();
                    adapter.Fill(ds);
                    dataGridView1.DataSource = ds.Tables[0];
                }
            }
            else MessageBox.Show("Без данных, организацию найти не удается");
        }

        private void tbSearch_MouseHover(object sender, EventArgs e)
        {
            toolTip1.SetToolTip(tbSearch, "Введите название компании или её ИНН");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OrgLoadData();
        }

        private void всехКромеСкрытыхToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OrgLoadData();
        }

        private void всехToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string sql = "SELECT * FROM Organizations WHERE Status=N'2'";
            OrgLoadData(sql);
        }

        private void Organizations_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (close)
            {
                Application.Exit();
            }
        }
    }
}
