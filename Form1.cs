using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.IO.Ports;
using System.Text;
namespace ESP_Mesh_creator
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            LoadAvailablePorts(); // ВЫзов метода для обнаружения доступных COM портов
            float new_size = 12.0f; // Размер символов в lable
            label1.Font = new Font(label1.Font.FontFamily, new_size, label1.Font.Style); // Задает размер label
            label2.Font = new Font(label2.Font.FontFamily, new_size, label2.Font.Style); // Задает размер label
            label3.Font = new Font(label3.Font.FontFamily, new_size, label3.Font.Style); // Задает размер label
            label4.Font = new Font(label4.Font.FontFamily, new_size, label4.Font.Style); // Задает размер label
            label5.Font = new Font(label5.Font.FontFamily, new_size, label5.Font.Style); // Задает размер label
        }
        private void LoadAvailablePorts() // Метод подгрузки COM-портов
        {
            string[] ports = SerialPort.GetPortNames();// Получаем список доступных COM портов   
            comboBox1.Items.Clear();// Очищаем ComboBox перед загрузкой
            if (ports.Length > 0)// Если порты есть, добавляем их в ComboBox
            {
                comboBox1.Items.AddRange(ports);
                comboBox1.SelectedIndex = 0; // По умолчанию выбираем первый доступный порт
            }
            else
            {
                MessageBox.Show("Нет доступных COM портов.");
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != "" 
                &&  textBox2.Text != "" 
                && numericUpDown1.Value >= numericUpDown1.Minimum && numericUpDown1.Value <= numericUpDown1.Maximum 
                && numericUpDown2.Value >= numericUpDown2.Minimum && numericUpDown2.Value <= numericUpDown2.Maximum
                && Convert.ToInt32(textBox1.Text.Length.ToString()) >= 8 
                && Convert.ToInt32(textBox2.Text.Length.ToString()) >= 8
                && Convert.ToInt32(textBox1.Text.Length.ToString()) <= 40
                && Convert.ToInt32(textBox2.Text.Length.ToString()) <= 40) // Проверка корректности ввода данных
            {
                if (checkBox1.Checked == false)
                {
                    string path = "work_code_arduino.txt";
                    string codeFromFile = File.ReadAllText(path, Encoding.GetEncoding(1251));
                    textBox5.Text = "";
                    textBox5.Text = "#include " + '\u0022' + "painlessMesh.h" + '\u0022'
                       + Environment.NewLine + "#define MESH_PREFIX " + '\u0022' + textBox1.Text + '\u0022'
                       + Environment.NewLine + "#define MESH_PASSWORD " + '\u0022' + textBox2.Text + '\u0022'
                       + Environment.NewLine + "#define MESH_PORT " + Convert.ToString(numericUpDown1.Value)
                       + Environment.NewLine + Environment.NewLine + "painlessMesh mesh"
                       + Environment.NewLine + "const uint8_t NODE_ID = " + Convert.ToString(numericUpDown2.Value)
                       + Environment.NewLine + codeFromFile; // Ввод в textBox5 данных полученных их полей ввода
                }
                else
                {
                    textBox5.Text = "";
                    textBox5.Text = "#include " + '\u0022' + "painlessMesh.h" + '\u0022'
                       + Environment.NewLine + "#define MESH_PREFIX " + '\u0022' + textBox1.Text + '\u0022'
                       + Environment.NewLine + "#define MESH_PASSWORD " + '\u0022' + textBox2.Text + '\u0022'
                       + Environment.NewLine + "#define MESH_PORT " + Convert.ToString(numericUpDown1.Value)
                       + Environment.NewLine + Environment.NewLine + "painlessMesh mesh"
                       + Environment.NewLine + "const uint8_t NODE_ID = " + Convert.ToString(numericUpDown2.Value)
                       + Environment.NewLine + textBox4.Text; // Ввод в textBox4 данных полученных их полей ввода и выбранного кода 
                }
                string code = textBox5.Text; // Пресвоение переменной текстаа из textBox5
                string fileName = "ArduinoCode.ino"; // Имя файла для Arduino
                textBox5.Visible = true;
                LoadAvailablePorts();
                string textToSend = textBox5.Text;
                if (comboBox1.SelectedIndex == -1)
                {
                    MessageBox.Show("Выберите COM порт.");// Проверяем, выбран ли COM порт
                    return;
                }
                string selectedPort = comboBox1.SelectedItem.ToString();// Получаем выбранный COM порт
                if (string.IsNullOrEmpty(textToSend)) // Проверка на пустой текст в TextBox
                {
                    MessageBox.Show("Отсутсвует код для записи");
                    return;
                }
                try
                {
                    using (SerialPort serialPort = new SerialPort(selectedPort, 9600))// Открываем COM порт, указываем порт и скорость передачи данных
                    {
                        serialPort.Open();                   
                        serialPort.WriteLine(textToSend);// Отправляем текст на Arduino
                        serialPort.Close();// Закрываем порт после отправки
                    }
                    MessageBox.Show("Текст отправлен успешно.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}"); // Вывод уведомления в случае ошибки 
                }
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != ""
                && textBox2.Text != ""
                && numericUpDown1.Value >= numericUpDown1.Minimum && numericUpDown1.Value <= numericUpDown1.Maximum
                && numericUpDown2.Value >= numericUpDown2.Minimum && numericUpDown2.Value <= numericUpDown2.Maximum
                && Convert.ToInt32(textBox1.Text.Length.ToString()) >= 8
                && Convert.ToInt32(textBox2.Text.Length.ToString()) >= 8
                && Convert.ToInt32(textBox1.Text.Length.ToString()) <= 40
                && Convert.ToInt32(textBox2.Text.Length.ToString()) <= 40) // Проверка корректности ввода данных
            {
                if (checkBox1.Checked == false)
                {
                    string path = "work_code_arduino.txt";
                    string codeFromFile = File.ReadAllText(path, Encoding.GetEncoding(1251)); // Считываем текс из файла, в том числе кириллицу
                    textBox5.Text = "";
                    textBox5.Text = "#include " + '\u0022' + "painlessMesh.h" + '\u0022'
                       + Environment.NewLine + "#define MESH_PREFIX " + '\u0022' + textBox1.Text + '\u0022'
                       + Environment.NewLine + "#define MESH_PASSWORD " + '\u0022' + textBox2.Text + '\u0022'
                       + Environment.NewLine + "#define MESH_PORT " + Convert.ToString(numericUpDown1.Value)
                       + Environment.NewLine + Environment.NewLine + "painlessMesh mesh"
                       + Environment.NewLine + "const uint8_t NODE_ID = " + Convert.ToString(numericUpDown2.Value)
                       + Environment.NewLine + codeFromFile; // Ввод в textBox5 данных полученных их полей ввода
                }
                else
                {
                    textBox5.Text = "";
                    textBox5.Text = "#include " + '\u0022' + "painlessMesh.h" + '\u0022'
                       + Environment.NewLine + "#define MESH_PREFIX " + '\u0022' + textBox1.Text + '\u0022'
                       + Environment.NewLine + "#define MESH_PASSWORD " + '\u0022' + textBox2.Text + '\u0022'
                       + Environment.NewLine + "#define MESH_PORT " + Convert.ToString(numericUpDown1.Value)
                       + Environment.NewLine + Environment.NewLine + "painlessMesh mesh"
                       + Environment.NewLine + "const uint8_t NODE_ID = " + Convert.ToString(numericUpDown2.Value)
                       + Environment.NewLine + textBox4.Text; // Ввод в textBox4 данных полученных их полей ввода и выбранного кода 
                }
                string code = textBox5.Text; // Сохранение текста в переменную
                string fileName = "ArduinoCode.ino"; // Имя файла для Arduino
                textBox5.Visible = true;
                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                {
                    saveFileDialog.Filter = "Arduino Files (*.ino)|*.ino|All Files (*.*)|*.*";
                    saveFileDialog.Title = "Сохранить код Arduino";
                    saveFileDialog.FileName = "ArduinoCode.ino"; // Имя файла по умолчанию
                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        try
                        {
                            // Сохраняем данные в выбранный файл
                            File.WriteAllText(saveFileDialog.FileName, code);
                            MessageBox.Show("Файл успешно сохранен!");
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Ошибка при сохранении файла: {ex.Message}");
                        }
                    }
                }
            }
        }
        private void textBox1_KeyPress(object sender, KeyPressEventArgs e) 
        {
            if ((e.KeyChar >= '0') && (e.KeyChar <= '9') || (e.KeyChar == 8) || (e.KeyChar == 95) || (e.KeyChar >= 65) && (e.KeyChar <= 90) || (e.KeyChar >= 97) && (e.KeyChar <= 122)) // Позволяет использовать только анлгийские буквы и символ "_" , а также стерать введенный текст
            {
                return;
            }
            if (Char.IsControl(e.KeyChar))
            {
                if (e.KeyChar == (char)(Keys.Enter))
                    textBox1.Focus();
            }
            e.Handled = true;
        }
        private void textBox2_KeyPress(object sender, KeyPressEventArgs e) 
        {
            if ((e.KeyChar >= '0') && (e.KeyChar <= '9') || (e.KeyChar == 8) || (e.KeyChar == 95) || (e.KeyChar >= 65) && (e.KeyChar <= 90) || (e.KeyChar >= 97) && (e.KeyChar <= 122)) // Позволяет использовать только анлгийские буквы и символ "_" , а также стерать введенный текст
            {
                return;
            }
            if (Char.IsControl(e.KeyChar))
            {
                if (e.KeyChar == (char)(Keys.Enter))
                    textBox1.Focus();
            }
            e.Handled = true;
        }
        private void textBox2_MouseClick(object sender, MouseEventArgs e)
        {}
        private void button3_Click(object sender, EventArgs e)
        {
            textBox4.Text = "";
            OpenFileDialog openfiledialog1 = new OpenFileDialog(); // Создаем диалоговое окно для выбора файла с кодом в формате .txt 
            openfiledialog1.Title = "Выберете файл";
            openfiledialog1.Filter = "текстовый файлы (*.txt)|*.txt| Все файлы| *.*";
            openfiledialog1.Multiselect = false;
            if (openfiledialog1.ShowDialog() == DialogResult.OK)
            {
                string path = openfiledialog1.FileName;
                textBox4.Text = System.IO.File.ReadAllText(path, Encoding.GetEncoding(1251));// Считываем текс из файла, в том числе кириллицу
            }
        }
        private void button4_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Доступные переменные для работы:" + Environment.NewLine +
                "'MESH_PREFIX' - имя mesh сети;" + Environment.NewLine +
                "'MESH_PASSWORD' - пароль mesh сети;" + Environment.NewLine +
                "MESH_PORT' - порт mesh сети;" + Environment.NewLine +
                "NODE_ID' - ID узла.", "Информация",
                                 MessageBoxButtons.OK,
                                 MessageBoxIcon.Information); // Вызывает MessageBox с ифнормацией для пользователя
        }
        

    }
}