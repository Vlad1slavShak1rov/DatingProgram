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
using Path = System.IO.Path;

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

        private BitmapImage currentImage;

        private Client client;
        public DatingForm DatingForm { get; set;}
        public Characteristic Characteristic { get; set;}

        bool IsUpdate;
        public DattingFormWindow(Client client, bool isUpdate = false)
        {
            if(client == null)
            {
                MessageBox.Show("Вы не авторизованы в системе!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            InitializeComponent();

            this.client = client;

            using var context = new MyDbContext();
            DatingForm = context.DatingForms.FirstOrDefault(d => d.ClientId == client.Id);

            // Загрузка данных анкеты, если она уже существует
            if (DatingForm != null) LoadData(context);
            IsUpdate = isUpdate;
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

            if (imagePath.Count > 0)
                DisplayImage(imagePath[0]);

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
        private void DisplayImage(string imagePath)
        {
            // Освобождаем предыдущее изображение
            if (currentImage != null)
            {
                currentImage = null;
            }

            // Очищаем текущее изображение
            myImage.Source = null;

            // Принудительная сборка мусора
            GC.Collect();
            GC.WaitForPendingFinalizers();

            // Загружаем новое изображение
            currentImage = LoadImageWithoutLock(imagePath);
            myImage.Source = currentImage;
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
            string age = tbAge.Text;
            string gender = rbMale.IsChecked == true ? "Мужчина" : "Женщина";
            string minAge = tbMinAge.Text;
            string maxAge = tbMaxAge.Text;
            string city = tbCity.Text;
            string description = tbAboutMe.Text;
            string purposeDating = (cbPurposeDating.SelectedItem as ComboBoxItem)?.Content.ToString();

            // Проверка заполнения всех полей
            if (string.IsNullOrEmpty(age) || string.IsNullOrEmpty(minAge) ||
                string.IsNullOrEmpty(maxAge) || string.IsNullOrEmpty(city) ||
                string.IsNullOrEmpty(description))
            {
                MessageBox.Show("Пожалуйста, заполните все поля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Проверка корректности возрастов
            if (!int.TryParse(age, out int ageValue) ||
                !int.TryParse(minAge, out int ageMinValue) ||
                !int.TryParse(maxAge, out int ageMaxValue))
            {
                MessageBox.Show("Введите корректные числовые значения возраста.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                ShowFirstPage();
                return;
            }

            if (ageValue < 18)
            {
                MessageBox.Show("Возраст должен быть не менее 18 лет.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (ageMinValue < 18 || ageMaxValue < 18 || ageMinValue > ageMaxValue)
            {
                MessageBox.Show("Некорректный диапазон возраста для поиска.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (imagePath == null || imagePath.Count == 0)
            {
                MessageBox.Show("Добавьте хотя бы одно фото.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            using var context = new MyDbContext();

            var existClient = context.Client.FirstOrDefault(c => c.Id == client.Id);
            if (existClient == null)
            {
                MessageBox.Show("Клиент не найден!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Получаем характеристику (если уже есть)
            var existCharacteristic = context.Characteristics.FirstOrDefault(c => c.Id == existClient.CharacteristicId);

            if (existCharacteristic == null)
            {
                existCharacteristic = new Characteristic()
                {
                    City = city,
                    Gender = gender,
                    Age = ageValue,
                };
                context.Characteristics.Add(existCharacteristic);
                context.SaveChanges();
                existClient.CharacteristicId = existCharacteristic.Id;
            }
            else
            {
                // Обновляем поля характеристики
                existCharacteristic.Age = ageValue;
                existCharacteristic.City = city;
                existCharacteristic.Gender = gender;
            }


            // Получаем анкету (если есть)
            var existForm = context.DatingForms.FirstOrDefault(df => df.ClientId == client.Id);
            if (existForm == null)
            {
                existForm = new DatingForm
                {
                    ClientId = existClient.Id,
                    DateCreated = DateTime.Now.Date
                };
                context.DatingForms.Add(existForm);
            }
            // Обновляем поля анкеты
            existForm.MinAge = ageMinValue;
            existForm.MaxAge = ageMaxValue;
            existForm.Description = description;
            existForm.PurposeDating = purposeDating;

            // Удаляем старые фото клиента и добавляем новые
            var oldPhotos = context.ClientPhotos.Where(cp => cp.ClientId == client.Id).ToList();
            context.ClientPhotos.RemoveRange(oldPhotos);

            foreach (var path in imagePath)
            {
                context.ClientPhotos.Add(new ClientPhoto
                {
                    ClientId = client.Id,
                    Path = path,
                    DateAdded = DateTime.Now.Date
                });
            }

            context.SaveChanges();
            MessageBox.Show("Анкета успешно сохранена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

            DatingForm = existForm;
            this.DialogResult = true;
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

        // Событие нажатия кнопки "Добавить фото"
        private void btAddPhoto_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Filter = "Image Files (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg";
            bool? result = dlg.ShowDialog();

            if (result == true)
            {
                string projectRoot = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\.."));
                string resourceFolderPath = Path.Combine(projectRoot, $"Resource", $"MyProfile_{client.Id}");

                if (!Directory.Exists(resourceFolderPath))
                    Directory.CreateDirectory(resourceFolderPath);

                string selectedFilePath = dlg.FileName;
                string fileName = Path.GetFileName(selectedFilePath);
                string destinationFilePath = Path.Combine(resourceFolderPath, fileName);

                try
                {
                    // Используем временный MemoryStream, чтобы освободить исходный файл
                    byte[] fileBytes = File.ReadAllBytes(selectedFilePath);

                    // Перезаписываем
                    File.WriteAllBytes(destinationFilePath, fileBytes);

                    imagePath.Add(destinationFilePath);

                    imagePageCounter = 0;
                    DisplayImage(destinationFilePath);
                    UpdateTextBlockCounter();

                    MessageBox.Show("Фото успешно добавлено!");
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
            imagePageCounter = (imagePageCounter + 1) % imagePath.Count;

            // Обновление источника изображения с учетом количества изображений
            DisplayImage(imagePath[imagePageCounter]);
            UpdateTextBlockCounter();
        }

        // Событие нажатия кнопки "Назад" для просмотра изображений
        private void btLeftImage_Click(object sender, RoutedEventArgs e)
        {
            if (imagePath.Count == 0) return;

            imagePageCounter = (imagePageCounter - 1 + imagePath.Count) % imagePath.Count;
            DisplayImage(imagePath[imagePageCounter]);
            UpdateTextBlockCounter();
        }

        // Событие нажатия кнопки "Удалить фото"
        private void btDeletePhoto_Click(object sender, RoutedEventArgs e)
        {
            if (imagePath.Count == 0) return;

            string currentPath = imagePath[imagePageCounter];

            // Освобождаем изображение перед удалением
            myImage.Source = null;
            if (currentImage != null)
            {
                currentImage = null;
            }

            // Принудительная сборка мусора
            GC.Collect();
            GC.WaitForPendingFinalizers();

            try
            {
                /*
                // Удаляем файл
                if (File.Exists(currentPath))
                {
                    File.Delete(currentPath);
                }
                */
                // Удаляем из списка
                imagePath.RemoveAt(imagePageCounter);

                // Обновляем счетчик
                if (imagePath.Count > 0)
                {
                    imagePageCounter = Math.Min(imagePageCounter, imagePath.Count - 1);
                    DisplayImage(imagePath[imagePageCounter]);
                }
                else
                {
                    imagePageCounter = 0;
                    myImage.Source = null;
                }

                // Удаляем из базы данных
                using var context = new MyDbContext();
                var photoToDelete = context.ClientPhotos.FirstOrDefault(cp => cp.ClientId == client.Id && cp.Path == currentPath);
                if (photoToDelete != null)
                {
                    context.ClientPhotos.Remove(photoToDelete);
                    context.SaveChanges();
                }

                UpdateTextBlockCounter();
                MessageBox.Show("Фото успешно удалено!");
            }
            catch (IOException ioEx)
            {
                MessageBox.Show($"Файл занят другим процессом: {ioEx.Message}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении файла: {ex.Message}");
            }
        }

        // Загрузка изображения без блокировки файла
        private BitmapImage LoadImageWithoutLock(string path)
        {
            if (!File.Exists(path)) return null;
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

        // Обновление счетчика изображений
        private void UpdateTextBlockCounter()
        {
            tbCountImagePage.Text = $"{(imagePath.Count == 0 ? 0 : imagePageCounter + 1)} / {imagePath.Count}";
        }
    }
}
