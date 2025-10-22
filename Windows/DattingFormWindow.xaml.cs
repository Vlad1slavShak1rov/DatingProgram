using DatingProgram.DB;
using DatingProgram.Models;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DatingProgram.Windows
{
    /// <summary>
    /// Логика взаимодействия для DattingFormWindow.xaml
    /// </summary>
    public partial class DattingFormWindow : Window
    {
        //Счетчик страниц
        int pageCounter = 0;

        //Максимальное количество страниц
        int maxPageCounter = 2;

        //Счетчик изображений
        int imagePageCounter = 0;

        //Пути к изображениям
        List<string> imagePath = new();

        private Client client;
        public DatingForm DatingForm { get; set;}
        public Characteristic Characteristic { get; set;}
        public DattingFormWindow(Client client)
        {
            InitializeComponent();

            this.client = client;

            using var context = new MyDbContext();
            DatingForm = context.DatingForms.FirstOrDefault(d=>d.ClientId == client.Id);

            // Загрузка данных анкеты, если она уже существует
            if (DatingForm != null) LoadData(context);
        }

        // Загрузка данных анкеты из БД
        private void LoadData(MyDbContext context)
        {
            // Получение данных клиента и его фотографий
            var clientPhotos = context.ClientPhotos.Where(cp => cp.ClientId == client.Id).ToList();

            // Получение характеристики клиента
            Characteristic = context.Characteristics.FirstOrDefault(c => c.Id == client.CharacteristicId);

            // Заполнение полей формы
            if (Characteristic.Gender == "Мужчина")
            {
                rbMale.IsChecked = true;
                rbFemale.IsChecked = false;
            }
            else
            {
                rbFemale.IsChecked = true;
                rbMale.IsChecked = false;
            }

            // Заполнение списка с фотографиями
            foreach (var p in clientPhotos)
            {
                imagePath.Add(p.Path);
            }

            cbPurposeDating.SelectedItem = cbPurposeDating.Items.Cast<ComboBoxItem>()
                .FirstOrDefault(i => i.Content.ToString() == DatingForm.PurposeDating);

            if(imagePath.Count >0)
                myImage.Source = LoadImageWithoutLock(imagePath[0]);

            UpdateTextBlockCounter();
            DataContext = this;
        }

        // Переход на следующую страницу
        private void btRight_Click(object sender, RoutedEventArgs e)
        {
            ChangedPage(1);
        }

        // Переход на предыдущую страницу
        private void btLeft_Click(object sender, RoutedEventArgs e)
        {
            ChangedPage(-1);
        }

        // Изменение страницы (вперед/назад)
        private void ChangedPage(int k )
        {
            if (maxPageCounter != 0)
            {
                pageCounter += 1 *k;
                SelectPage();
            }
        }

        // Выбор страницы для отображения
        private void SelectPage()
        {
            switch (pageCounter)
            {
                case 0:
                    {
                        ShowFirstPage();
                        break;
                    }
                case 1:
                    {
                        ShowSecondPage();
                        break;
                    }
                case 2:
                    {
                        ShowLastPage();
                        break;
                    }
            }
        }

        // Событие нажатия кнопки "Сохранить" в форме знакомств 
        private void btSave_Click(object sender, RoutedEventArgs e)
        {
            // Сбор данных из формы
            string age = tbAge.Text;
            string gender = rbMale.IsChecked == true ? "Мужчина" : "Женщина";
            string minAge = tbMinAge.Text;
            string maxAge = tbMaxAge.Text;
            string city = tbCity.Text;
            string description = tbAboutMe.Text;
            string purposeDating = (cbPurposeDating.SelectedItem as ComboBoxItem).Content.ToString();

            // Проверка заполнения всех полей
            if (string.IsNullOrEmpty(age) || string.IsNullOrWhiteSpace(minAge) 
               || string.IsNullOrEmpty(maxAge) || string.IsNullOrWhiteSpace(city) 
               || string.IsNullOrEmpty(city) || string.IsNullOrWhiteSpace(description) )
            {
                MessageBox.Show("Пожалуйста, заполните все поля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Проверка корректности введенных значений
            int ageValue, ageMaxValue, ageMinValue;
            if(!int.TryParse(age, out ageValue) || !int.TryParse(minAge, out ageMinValue) 
               || !int.TryParse(maxAge, out ageMaxValue))
            {
                MessageBox.Show("Пожалуйста, введите корректные числовые значения для возраста.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                ShowFirstPage();
                return;
            }

            // Проверка возрастных ограничений
            if (ageValue < 18)
            {
                MessageBox.Show("Возраст должен быть не менее 18 лет.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                ShowFirstPage();
                return;
            }

            // Проверка диапазона возраста для поиска
            if (ageMinValue < 18 || ageMaxValue < 18 || ageMinValue > ageMaxValue)
            {
                ShowSecondPage();
                MessageBox.Show("Пожалуйста, введите корректные значения для диапазона возраста.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Проверка наличия хотя бы одного изображения
            if (imagePath.IsNullOrEmpty())
            {
                MessageBox.Show("Пожалуйста, добавьте хотя бы одно изображение.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Сохранение данных формы знакомств

            using var context = new MyDbContext();

            //Получаем клиента с отслеживанием
            var existClient = context.Client.FirstOrDefault(c => c.Id == client.Id);

            if (existClient == null)
            {
                MessageBox.Show("Клиент не найден в базе данных!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            //Создаём характеристику
            var characteristic = new Characteristic
            {
                Age = ageValue,
                City = city,
                Gender = gender
            };

            //Добавляем характеристику и сохраняем
            context.Characteristics.Add(characteristic);
            context.SaveChanges();

            //Привязываем к клиенту
            existClient.CharacteristicId = characteristic.Id;
            context.SaveChanges();

            //Создаём анкету
            var datingForm = new DatingForm
            {
                Client = existClient,
                MinAge = ageMinValue,
                MaxAge = ageMaxValue,
                Description = description,
                PurposeDating = purposeDating,
                DateCreated = DateTime.Now.Date
            };
            context.DatingForms.Add(datingForm);

            //Добавляем фото
            foreach (var path in imagePath)
            {
                context.ClientPhotos.Add(new ClientPhoto
                {
                    ClientId = existClient.Id,
                    Path = path,
                    DateAdded = DateTime.Now.Date,
                });
            }

            context.SaveChanges();
            MessageBox.Show("Анкета успешно сохранена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close();
        }

        // Отображение первой страницы
        private void ShowFirstPage()
        {
            // Настройка доступа кнопок навигации по страницам
            btLeft.IsEnabled = false;
            btRight.IsEnabled = true;

            //Отображение нужной страницы
            spFirst.Visibility = Visibility.Visible;
            spSecond.Visibility = Visibility.Collapsed;
            spThird.Visibility = Visibility.Collapsed;
        }
        
        //Отображение второй страницы
        private void ShowSecondPage()
        {
            //Настройка доступа кнопок навигации по страницам
            btLeft.IsEnabled = true;
            btRight.IsEnabled = true;

            //Отображение нужной страницы
            spFirst.Visibility = Visibility.Collapsed;
            spSecond.Visibility = Visibility.Visible;
            spThird.Visibility = Visibility.Collapsed;
        }

        //Отображение последней страницы
        private void ShowLastPage()
        {
            //Настройка доступа кнопок навигации по страницам
            btLeft.IsEnabled = true;
            btRight.IsEnabled = false;

            //Отображение нужной страницы
            spFirst.Visibility = Visibility.Collapsed;
            spSecond.Visibility = Visibility.Collapsed;
            spThird.Visibility = Visibility.Visible;
        }

        // Событие выбора радиокнопки (мужчина/женщина)
        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (rbMale == null || rbFemale == null) return;
                RadioButton radioButton = (sender as RadioButton);

            // Определение выбранного пола и установка соответствующей радиокнопки в форме знакомств
            if (radioButton.Tag.ToString() == "m")
            {
                
                rbMale.IsChecked = true;
                rbFemale.IsChecked = false;
            }
            else
            {
                rbMale.IsChecked = false;
                rbFemale.IsChecked = true;
            }
        }

        private void btAddPhoto_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Filter = "Image Files (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg";
            bool? result = dlg.ShowDialog();

            if (result == true)
            {
                // получаем корень проекта
                string projectRoot = System.IO.Path.GetFullPath(
                    System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..")
                );

                // путь к папке Resource в корне проекта
                string resourceFolderPath = System.IO.Path.Combine(projectRoot, $"Resource", $"MyProfile_{client.Id}");

                // создаем папку если не существует
                if (!Directory.Exists(resourceFolderPath))
                {
                    Directory.CreateDirectory(resourceFolderPath);
                }

                // копируем выбранный файл в папку Resource
                string selectedFilePath = dlg.FileName;
                string fileName = System.IO.Path.GetFileName(selectedFilePath);
                string destinationFilePath = System.IO.Path.Combine(resourceFolderPath, fileName);

                // Копирование файла с обработкой ошибок
                try
                {
                    File.Copy(selectedFilePath, destinationFilePath, true);
                    MessageBox.Show("Фото успешно добавлено!");

                    imagePath.Add(destinationFilePath);
                    myImage.Source = LoadImageWithoutLock(imagePath[imagePath.Count - 1]);
                    UpdateTextBlockCounter();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при копировании файла: {ex.Message}");
                }
            }
        }

        // Событие нажатия кнопки "Вперед" для просмотра изображений
        private void btRightImage_Click(object sender, RoutedEventArgs e)
        {
            if (imagePath.Count == 0) return;
            Math.Abs(imagePageCounter++);

            // Обновление источника изображения с учетом количества изображений
            myImage.Source = LoadImageWithoutLock(imagePath[Math.Abs(imagePageCounter) % imagePath.Count]);
            UpdateTextBlockCounter();
        }

        // Событие нажатия кнопки "Назад" для просмотра изображений
        private void btLeftImage_Click(object sender, RoutedEventArgs e)
        {
            if (imagePath.Count == 0) return;
            Math.Abs(imagePageCounter--);
            myImage.Source = LoadImageWithoutLock(imagePath[Math.Abs(imagePageCounter) % imagePath.Count]);
            UpdateTextBlockCounter();
        }

        // Событие нажатия кнопки "Удалить фото"
        private void btDeletePhoto_Click(object sender, RoutedEventArgs e)
        {
            string currentPath = imagePath[imagePageCounter % imagePath.Count];
            imagePath.Remove(currentPath);

            // Обновление отображаемого изображения после удаления
            if (imagePath.Count > 0)
            {
                myImage.Source = LoadImageWithoutLock(imagePath[0]);
            }
            else
            {
                myImage.Source = null;
            }

            File.Delete(currentPath);

            using var context = new MyDbContext();
            var photoToDelete = context.ClientPhotos.FirstOrDefault(cp => cp.ClientId == client.Id && cp.Path == currentPath);
            if (photoToDelete != null)
            {
                context.ClientPhotos.Remove(photoToDelete);
                context.SaveChanges();
            }


            UpdateTextBlockCounter();
        }

        // Загрузка изображения без блокировки файла
        private BitmapImage LoadImageWithoutLock(string path)
        {
            var bitmap = new BitmapImage();
            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad; 
                bitmap.UriSource = null;
                bitmap.StreamSource = stream;
                bitmap.EndInit();
            }
            bitmap.Freeze();
            return bitmap;
        }

        private void UpdateTextBlockCounter()
        {
            tbCountImagePage.Text = $"{(imagePath.Count == 0 ? 0 : (imagePageCounter % imagePath.Count) + 1)} / {imagePath.Count}";
        }
    }
}
