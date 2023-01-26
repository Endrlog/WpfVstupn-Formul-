using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using Microsoft.Win32;
using System.IO;
using System.Text.RegularExpressions;

namespace WpfVstupníFormulář
{
    class Person : INotifyPropertyChanged
    {
        private string _Name, _Surname;
        private int _Birth;

        public event PropertyChangedEventHandler PropertyChanged;

        public string Name
        {
            get { return _Name; }
            set
            {
                _Name = value;
                OnPropertyChanged("Name");
                OnPropertyChanged("Status");
            }
        }
        public string Surname
        {
            get { return _Surname; }
            set
            {
                _Surname = value;
                OnPropertyChanged("Surname");
                OnPropertyChanged("Status");
            }
        }
        public int Birth
        {
            get { return _Birth; }
            set
            {
                _Birth = value;
                OnPropertyChanged("Birth");
                OnPropertyChanged("Status");
            }
        }

        public virtual string Status
        {
            get { return Name + " " + Surname + " " + Birth; }
        }

        public override string ToString()
        {
            return Name + " " + Surname + " " + Birth;
        }

        // pomocná metoda pro informaci o změně v datech
        public void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null) // jestli někdo poslouchá ...
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }
    }

    class Employee : Person
    {
        private string _Education;
        private string _Position;
        private int _Pay;

        public string Education
        {
            get { return _Education; }
            set
            {
                _Education = value;
                OnPropertyChanged("Education");
                OnPropertyChanged("Status");
            }
        }
        public string Position
        {
            get { return _Position; }
            set
            {
                _Position = value;
                OnPropertyChanged("Position");
                OnPropertyChanged("Status");
            }
        }
        public int Pay
        {
            get { return _Pay; }
            set
            {
                _Pay = value;
                OnPropertyChanged("Pay");
                OnPropertyChanged("Status");
            }
        }

        public override string Status
        {
            get { return $"{Name} {Surname} {Birth} {Education} {Position} {Pay}"; }
        }

        public override string ToString()
        {
            return $@"{Name} {Surname} {Birth} {Education} {Position} {Pay},
";
        }
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        Employee emp;
        StringBuilder output = new StringBuilder();
        bool isLoading = false;
        TaskCompletionSource<bool> tcs;
        bool isNameCorrect = false;
        bool isSurnameCorrect = false;
        bool isBirthCorrect = true;
        bool isWorkPositionCorrect = false;
        bool isPayCorrect = true;
        public MainWindow()
        {
            InitializeComponent();

            DataContext = emp = new Employee()
            {
                Birth = DateTime.Now.Year,
                Education = "Základní"
            };
        }

        private void btAdd_Click(object sender, RoutedEventArgs e)
        {
            if (isNameCorrect && isSurnameCorrect && isBirthCorrect && isWorkPositionCorrect && isPayCorrect)
            {
                output.Append(emp.ToString());
                emp.Name = "";
                emp.Surname = "";
                emp.Birth = DateTime.Now.Year;
                emp.Education = "Základní";
                emp.Position = "";
                emp.Pay = 0;
                isNameCorrect = false;
                isSurnameCorrect = false;
                isBirthCorrect = true;
                isWorkPositionCorrect = false;
                isPayCorrect = true;
                rctName.Visibility = Visibility.Visible;
                rctSurname.Visibility = Visibility.Visible;
                rctBirth.Visibility = Visibility.Hidden;
                rctWorkPosition.Visibility = Visibility.Visible;
                rctPay.Visibility = Visibility.Hidden;
                btAdd.Visibility = Visibility.Hidden;

                string[] breakpoints = { "," , @"
" };
                lsvOutput.ItemsSource = output.ToString().Split(breakpoints, StringSplitOptions.RemoveEmptyEntries);

                if (isLoading)
                {
                    tcs.SetResult(false);
                }
            }
        }

        private void btRemove_Click(object sender, RoutedEventArgs e)
        {
            emp.Name = "";
            emp.Surname = "";
            emp.Birth = DateTime.Now.Year;
            emp.Education = "Základní";
            emp.Position = "";
            emp.Pay = 0;
            tbName_LostFocus(tbName, new RoutedEventArgs());
            tbSurname_LostFocus(tbSurname, new RoutedEventArgs());
            tbBirth_LostFocus(tbBirth, new RoutedEventArgs());
            tbWorkPosition_LostFocus(tbWorkPosition, new RoutedEventArgs());
            tbPay_LostFocus(tbPay, new RoutedEventArgs());
            if (isLoading)
            {
                tcs.SetResult(false);
            }
        }

        private void BtSave_Click(object sender, RoutedEventArgs e)
        {
            string fileText = output.ToString();
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.FileName = "Document";
            dlg.DefaultExt = ".txt";
            dlg.Filter = "Text documents (.txt)|*.txt";
            dlg.FilterIndex = 2;

            if (dlg.ShowDialog() == true)
            {
                File.WriteAllText(dlg.FileName, fileText);
            }
            output = new StringBuilder();
            lsvOutput.ItemsSource = new string[0];
            btRemove_Click(btRemove, new RoutedEventArgs());
        }

        private async void btLoad_Click(object sender, RoutedEventArgs e)
        {
            isLoading = true;
            output = new StringBuilder();
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Text documents (.txt)|*.txt";
            dlg.FilterIndex = 2;
            lsvOutput.ItemsSource = new string[0];

            if (dlg.ShowDialog() == true)
            {
                string[] breakpoints = { " ","," };

                foreach (string line in File.ReadAllLines(dlg.FileName))
                {
                    string[] data = line.Split(breakpoints, StringSplitOptions.RemoveEmptyEntries);
                    emp.Name = data[0];
                    emp.Surname = data[1];
                    emp.Birth = Convert.ToInt32(data[2]);
                    emp.Education = data[3];
                    emp.Position = data[4];
                    emp.Pay = Convert.ToInt32(data[5]);
                    tbName_LostFocus(tbName, new RoutedEventArgs());
                    tbSurname_LostFocus(tbSurname, new RoutedEventArgs());
                    tbBirth_LostFocus(tbBirth, new RoutedEventArgs());
                    tbWorkPosition_LostFocus(tbWorkPosition, new RoutedEventArgs());
                    tbPay_LostFocus(tbPay, new RoutedEventArgs());
                    tcs = new TaskCompletionSource<bool>();
                    await tcs.Task;
                }
            }
            isLoading = false;
        }

        private void tbName_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            Regex reg = new Regex("^[a-zA-ZáčďéěíňóřšťůúýžÁČĎÉĚÍŇÓŘŠŤŮÚÝŽ]+$");
            if (tb.Text == "")
            {
                rctName.Visibility = Visibility.Visible;
                isNameCorrect = false;
                tblNameError.Text = "Musí být zadáno";
            }
            else if (reg.IsMatch(tb.Text))
            {
                rctName.Visibility = Visibility.Hidden;
                isNameCorrect = true;
                tblNameError.Text = "";
            }
            else
            {
                rctName.Visibility = Visibility.Visible;
                isNameCorrect = false;
                tblNameError.Text = "Jméno může obsahovat jen písmena";
            }
            ShowHideBtAdd();
        }

        private void tbSurname_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            Regex reg = new Regex("^[a-zA-ZáčďéěíňóřšťůúýžÁČĎÉĚÍŇÓŘŠŤŮÚÝŽ]+$");
            if (tb.Text == "")
            {
                rctSurname.Visibility = Visibility.Visible;
                isSurnameCorrect = false;
                tblSurnameError.Text = "Musí být zadáno";
            }
            else if (reg.IsMatch(tb.Text))
            {
                rctSurname.Visibility = Visibility.Hidden;
                isSurnameCorrect = true;
                tblSurnameError.Text = "";
            }
            else
            {
                rctSurname.Visibility = Visibility.Visible;
                isSurnameCorrect = false;
                tblSurnameError.Text = "Příjmení může obsahovat jen písmena";
            }
            ShowHideBtAdd();
        }

        private void tbBirth_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            Regex reg = new Regex("^[0-9][0-9][0-9][0-9]$");
            if (tb.Text == "")
            {
                rctBirth.Visibility = Visibility.Visible;
                isBirthCorrect = false;
                tblBirthError.Text = "Musí být zadáno";
            }
            else if (reg.IsMatch(tb.Text))
            {
                rctBirth.Visibility = Visibility.Hidden;
                isBirthCorrect = true;
                tblBirthError.Text = "";
            }
            else
            {
                rctBirth.Visibility = Visibility.Visible;
                isBirthCorrect = false;
                tblBirthError.Text = "Rok narození musí obsahovat jen čtyři čísla";
            }
            ShowHideBtAdd();
        }

        private void tbWorkPosition_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            Regex reg = new Regex("^[a-zA-ZáčďéěíňóřšťůúýžÁČĎÉĚÍŇÓŘŠŤŮÚÝŽ]+$");
            if (tb.Text == "")
            {
                rctWorkPosition.Visibility = Visibility.Visible;
                isWorkPositionCorrect = false;
                tblWorkPositionError.Text = "Musí být zadáno";
            }
            else if (reg.IsMatch(tb.Text))
            {
                rctWorkPosition.Visibility = Visibility.Hidden;
                isWorkPositionCorrect = true;
                tblWorkPositionError.Text = "";
            }
            else
            {
                rctWorkPosition.Visibility = Visibility.Visible;
                isWorkPositionCorrect = false;
                tblWorkPositionError.Text = "Pracovní místo může obsahovat jen písmena";
            }
            ShowHideBtAdd();
        }

        private void tbPay_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            Regex reg = new Regex("^[0-9]+$");
            if (tb.Text == "")
            {
                rctPay.Visibility = Visibility.Visible;
                isPayCorrect = false;
                tblPayError.Text = "Musí být zadáno";
            }
            else if (reg.IsMatch(tb.Text))
            {
                rctPay.Visibility = Visibility.Hidden;
                isPayCorrect = true;
                tblPayError.Text = "";
            }
            else
            {
                rctPay.Visibility = Visibility.Visible;
                isPayCorrect = false;
                tblPayError.Text = "Mzda může obsahovat jen čísla";
            }
            ShowHideBtAdd();
        }

        private void ShowHideBtAdd()
        {
            if (isNameCorrect && isSurnameCorrect && isBirthCorrect && isWorkPositionCorrect && isPayCorrect)
            {
                btAdd.Visibility = Visibility.Visible;
            }
            else
            {
                btAdd.Visibility = Visibility.Hidden;
            }
        }
    }
}
