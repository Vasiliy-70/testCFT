using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{

    public partial class Form1 : Form
    {
        private int mode = 0;
        //mode == 0 - просмотр;
        //mode == 1 - создание;
        //mode == 2 - редактирование;
        //mode == 3 - режим выбора нескольких позиций;
        private int indexCar;
        private Image tempImage; //Временная переменная для изображения
        private string pathNewImage; //Путь к новому изображению
        private string commandText; //Команда для БД

        //Заголовки параметров в таблице dataGridView
        private string makeHeader = "Марка";
        private string modelHeader = "Модель";
        private string yearHeader = "Год выпуска";
        private string bodyStyleHeader = "Тип кузова";

        List<Car> CarList = new List<Car>();//Список автомобилей
        //Загрузка данных с БД
        private void LoadFromDatabase()
        {
            using (SQLiteConnection Database = new SQLiteConnection("Data Source=Cars.db; Version=3;"))
            {
                try
                {
                    Database.Open();
                    //Проверяем существование таблицы
                    commandText = "SELECT Id, Make, Model, Year, BodyStyle, Image  FROM Cars";
                    SQLiteCommand Command = new SQLiteCommand(commandText, Database);
                    SQLiteDataReader reader = Command.ExecuteReader();
                    while (reader.Read())
                    {
                        Int32.TryParse(Convert.ToString(reader["Year"]), out int year);
                        CarList.Add(new Car(Convert.ToInt32(reader["Id"]), Convert.ToString(reader["Make"]), Convert.ToString(reader["Model"]), year, Convert.ToString(reader["BodyStyle"]), Convert.ToString(reader["Image"])));
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("База данных не найдена или повреждена и будет создана заново", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    //Удаляем существующую таблицу
                    commandText = "DROP TABLE IF EXISTS Cars";
                    SQLiteCommand Command = new SQLiteCommand(commandText, Database);
                    Command.ExecuteNonQuery();
                    //Создаем новую таблицу
                    commandText = "CREATE TABLE [Cars] ( [Id] INTEGER PRIMARY KEY AUTOINCREMENT, [Make] TEXT, [Model] TEXT, " +
                                             "[Year] INTEGER, [BodyStyle] TEXT, [Image] TEXT)";
                    Command = new SQLiteCommand(commandText, Database);
                    Command.ExecuteNonQuery();

                    //Заполняем по умолчанию
                    if (CarList.Count == 0)
                    {
                        CarList.Add(new Car(1, "Toyota", "Allion", 2009, "Седан", "C:\\Users\\Admin\\Desktop\\WindowsFormsApp1\\WindowsFormsApp1\\images\\1.jpg"));
                        CarList.Add(new Car(2, "Волга", "3110", 2001, "Седан", "C:\\Users\\Admin\\Desktop\\WindowsFormsApp1\\WindowsFormsApp1\\images\\2.jpg"));
                        CarList.Add(new Car(3, "Нива", "Урбан", 2016, "Хетчбек", "C:\\Users\\Admin\\Desktop\\WindowsFormsApp1\\WindowsFormsApp1\\images\\3.jpg"));
                    }
                    for (byte i = 0; i < CarList.Count; i++)
                    {
                        SaveIntoDatabase(CarList[i].Make, CarList[i].Model, CarList[i].Year, CarList[i].BodyStyle, CarList[i].Image);
                    }
                }
                Database.Close();
            }
        }
        //Сохранение данных с БД
        private void SaveIntoDatabase(string make, string model, int year, string bodyStyle, string image)
        {
            using (SQLiteConnection DataBase = new SQLiteConnection("Data Source=Cars.db; Version=3;"))
            {
                try
                {
                    DataBase.Open();
                    commandText = $"INSERT INTO Cars ([Make], [Model], [Year], [BodyStyle], [Image]) VALUES('{make}', '{model}', '{year}', '{bodyStyle}', '{image}');";
                    SQLiteCommand Command = new SQLiteCommand(commandText, DataBase);
                    Command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    LoadFromDatabase();
                }
                DataBase.Close();
            }
        }
        //Удаление данных с БД
        private void DeleteFromDatabase(int id)
        {
            using (SQLiteConnection Database = new SQLiteConnection("Data Source=Cars.db; Version=3;"))
            {
                try
                {
                    Database.Open();
                    commandText = $"DELETE FROM Cars WHERE id = {id};";
                    SQLiteCommand Command = new SQLiteCommand(commandText, Database);
                    Command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    LoadFromDatabase();
                }
                Database.Close();
            }

        }
        //Обновление данных в БД
        private void UpdateDatabase(int id, string make, string model, int year, string bodyStyle, string image)
        {
            using (SQLiteConnection Database = new SQLiteConnection("Data Source=Cars.db; Version=3;"))
            {
                try
                {
                    Database.Open();
                    commandText = $"UPDATE Cars SET Make = '{make}', Model = '{model}', Year = '{year}', BodyStyle = '{bodyStyle}', Image = '{image}' WHERE Id = '{id}';";
                    SQLiteCommand Command = new SQLiteCommand(commandText, Database);
                    Command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    LoadFromDatabase();
                }
                Database.Close();
            }
        }
        //Функция получения списка автомобилей
        private void GetCarList()
        {
            listView1.Clear();
            imageList1.Images.Clear();
            dataGridView1.Rows.Clear();
            CarList.Clear();
            dataGridView1.Visible = false;
            LoadFromDatabase();
            pictureBox1.Image = null;
            button_DeleteCar.Enabled = false;
            ListViewItem Car = new ListViewItem();
            imageList1.ImageSize = new Size(30, 30); //Задаем размер изображений для listView
            listView1.LargeImageList = imageList1; //Задаем библиотеку изображений

            //Заполняем библиотеку изображений, выводим базу автомобилей
            for (int i = 0; i < CarList.Count; i++)
            {
                try
                {
                    imageList1.Images.Add(Image.FromFile(CarList[i].Image));
                }
                catch
                {
                    imageList1.Images.Add(Properties.Resources.emptyImage);
                }
                ListViewItem listViewItem = new ListViewItem();
                listViewItem.Tag = i;
                listViewItem.Text = CarList[i].Make + " " + CarList[i].Model + "\n(" + CarList[i].Year + ")";
                listViewItem.ImageIndex = i;
                listView1.Items.Add(listViewItem);
            }
        }
        //Функция получения данных о выбранном автомобиле
        private void GetCarInfo(int indexCar)
        {
            dataGridView1.Enabled = true;
            dataGridView1.Rows.Clear();
            dataGridView1.Rows.Add(makeHeader, CarList[indexCar].Make);
            dataGridView1.Rows.Add(modelHeader, CarList[indexCar].Model);
            dataGridView1.Rows.Add(yearHeader, Convert.ToString(CarList[indexCar].Year));
            dataGridView1.Rows.Add(bodyStyleHeader, CarList[indexCar].BodyStyle);

            try
            {
                pictureBox1.Image = Image.FromFile(CarList[indexCar].Image);
            }
            catch
            {
                pictureBox1.Image = Properties.Resources.emptyImage;
            }
        }

        public Form1()
        {
            InitializeComponent();
            ChangeWorkMode(0);
            dataGridView1.AllowUserToResizeColumns = false;
            dataGridView1.AllowUserToResizeRows = false;
            dataGridView1.AllowUserToAddRows = false;
            openFileDialog1.Filter = "JPEG-files(*.jpg)|*.jpg";
            GetCarList();
        }

        //Получаем информацию о выбранном автомобиле
        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            button_DeleteCar.Enabled = false;
            dataGridView1.Visible = true;
            int count = listView1.SelectedItems.Count;
            if (count == 0)
            {
                ChangeWorkMode(0);
                dataGridView1.Rows.Clear();
                pictureBox1.Image = null;
                button_CancelChange.Enabled = false;
                dataGridView1.Visible = false;
            }
            if (count == 1)
            {
                ChangeWorkMode(0);
                indexCar = Convert.ToInt32(listView1.SelectedItems[0].Tag);
                GetCarInfo(indexCar);
                button_DeleteCar.Enabled = true;
            }
            if (count > 1)
            {
                ChangeWorkMode(3);
                button_DeleteCar.Enabled = true;
                dataGridView1.Visible = false;
            }
        }

        //Запрещаем выделение ячеек первого столбца
        private void DataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            for (byte i = 0; i < dataGridView1.Rows.Count; i++)
            {
                if (dataGridView1[0, i].Selected) dataGridView1.ClearSelection();
            }
        }

        //Кнопка добавления/изменения автомобиля
        private void Button1_Click(object sender, EventArgs e)
        {
            //Режим редактирования данных
            if (mode == 1 || mode == 2)
            {
                dataGridView1.ClearSelection();
                try
                {
                    bool error = false;
                    int year;
                    Int32.TryParse(dataGridView1[1, 2].Value.ToString(), out year);
                    string make = Convert.ToString(dataGridView1[1, 0].Value);
                    string model = Convert.ToString(dataGridView1[1, 1].Value);
                    string bodyStyle = Convert.ToString(dataGridView1[1, 3].Value);
                    string image = "";

                    for (int i = 0; i < dataGridView1.Rows.Count; i++)
                    {
                        if (Convert.ToString(dataGridView1.Rows[i].Cells[1].Value) == "")
                        {
                            dataGridView1.Rows[i].Cells[1].Style.BackColor = System.Drawing.Color.Red;
                            error = true;
                        }
                    }
                    if (error) throw new Exception("Заполнены не все поля");
                    if (year < 1884 || year > DateTime.Now.Year)
                    {
                        dataGridView1.Rows[2].Cells[1].Style.BackColor = System.Drawing.Color.Red;
                        throw new Exception($"Значение года должно быть в диапазоне от 1884 до {DateTime.Now.Year}");
                    }
                    if (mode == 1)
                    {
                        image = (pathNewImage != null) ? (pathNewImage) : ("");
                        CarList.Add(new Car(0, make, model, year, bodyStyle, image));
                        SaveIntoDatabase(make, model, year, bodyStyle, image);
                    }
                    if (mode == 2)
                    {
                        image = (pathNewImage != null) ? (pathNewImage) : (CarList[indexCar].Image);
                        CarList[indexCar].Id = CarList[indexCar].Id;
                        CarList[indexCar].Year = year;
                        CarList[indexCar].Make = make;
                        CarList[indexCar].Model = model;
                        CarList[indexCar].BodyStyle = bodyStyle;
                        CarList[indexCar].Image = image;
                        UpdateDatabase(CarList[indexCar].Id, CarList[indexCar].Make,
                            CarList[indexCar].Model, CarList[indexCar].Year,
                            CarList[indexCar].BodyStyle, CarList[indexCar].Image);
                    }

                    GetCarList();
                    dataGridView1.Rows.Clear();
                    pictureBox1.Image = null;
                    ChangeWorkMode(0);
                    return;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString(), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            //Перейти в режим добавления автомобиля
            if (mode == 0 || mode == 3)
            {
                if (listView1.SelectedItems.Count > 0)
                {
                    for (int i = 0; i < listView1.SelectedItems.Count; i++)
                    {
                        if (listView1.SelectedItems[i].Selected)
                        {
                            listView1.SelectedItems[i].Selected = false;
                            i--;
                        }

                    }
                }
                try
                {
                    pictureBox1.Image = Properties.Resources.emptyImage;
                }
                catch
                {
                    pictureBox1.Image = null;
                }
                dataGridView1.Rows.Clear();
                dataGridView1.Rows.Add(makeHeader, "");
                dataGridView1.Rows.Add(modelHeader, "");
                dataGridView1.Rows.Add(yearHeader, "");
                dataGridView1.Rows.Add(bodyStyleHeader, "");
                dataGridView1.ClearSelection();
                ChangeWorkMode(1);
            }
        }

        //Выбор режима работы (редактирование/создание)
        public void ChangeWorkMode(int _mode)
        {
            button_CancelChange.Enabled = false;
            button_AddSaveCar.Text = "Добавить автомобиль";
            button_DeleteCar.Text = "Удалить автомобиль";
            button_CancelChange.Text = "Отменить изменения";

            if (_mode == 0) //mode == 0 - просмотр;
            {
                mode = 0;
            }
            if (_mode == 1) //mode == 1 - создание;
            {
                mode = 1;
                button_AddSaveCar.Text = "Сохранить";
                button_CancelChange.Text = "Отменить";
                button_CancelChange.Enabled = true;
                dataGridView1.Visible = true;
            }
            if (_mode == 2) //mode == 2 - редактирование;
            {
                mode = 2;
                button_AddSaveCar.Text = "Принять изменения";
                button_CancelChange.Enabled = true;
            }
            if (_mode == 3) //mode == 3 - режим выбора нескольких позиций;
            {
                mode = 3;
                button_DeleteCar.Text = "Удалить автомобили";
                dataGridView1.Rows.Clear();
                dataGridView1.ClearSelection();
                pictureBox1.Image = null;
            }
            pathNewImage = null;
        }

        //Изменение значения параметра автомобиля
        private void DataGridView1_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            int index;
            if (dataGridView1.SelectedCells.Count > 0)
            {
                index = Convert.ToInt32(dataGridView1.SelectedCells[0].RowIndex);
                dataGridView1.Rows[index].Cells[1].Style.BackColor = System.Drawing.Color.White;
            }
            if (mode == 0)
            {
                ChangeWorkMode(2);
            }
        }

        //Вывод подскази при наведении на изображение
        private void PictureBox1_MouseEnter(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count == 0) return;
            tempImage = pictureBox1.Image;
            pictureBox1.Image = (mode == 1) ? (Properties.Resources.addImage) : (Properties.Resources.changeImage);
        }

        //Возвращение исходного изображения
        private void PictureBox1_MouseLeave(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count == 0) return;
            pictureBox1.Image = tempImage;
        }

        //Изменение изображения
        private void PictureBox1_Click(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count == 0) return;
            if (openFileDialog1.ShowDialog() == DialogResult.Cancel) return;
            if (mode == 0) ChangeWorkMode(2); //Перевод в режим редактирования
            pictureBox1.Image = Image.FromFile(openFileDialog1.FileName);
            pathNewImage = openFileDialog1.FileName;
        }
        //Кнопка удаления автомобиля
        private void Button2_Click(object sender, EventArgs e)
        {

            for (int i = 0; i < listView1.SelectedItems.Count; i++)
            {

                if (listView1.SelectedItems[i].Selected)
                {
                    indexCar = Convert.ToInt32(listView1.SelectedItems[i].Tag);
                    DeleteFromDatabase(CarList[indexCar].Id);
                    listView1.SelectedItems[i].Remove();
                    i--;
                }
            }
            GetCarList();
            ChangeWorkMode(0);
        }

        //Кнопка отмена изменений
        private void Button3_Click(object sender, EventArgs e)
        {
            if (mode == 1)
            {
                pictureBox1.Image = null;
                dataGridView1.Rows.Clear();
                dataGridView1.Enabled = false;
            }
            if (mode == 2) GetCarInfo(indexCar);

            ChangeWorkMode(0);
        }

        //Разблокируем dataGridView при добавлении строк
        private void DataGridView1_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            dataGridView1.Enabled = true;
        }

    }
}
