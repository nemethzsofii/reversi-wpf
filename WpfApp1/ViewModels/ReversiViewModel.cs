using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WpfApp1.Model;
using WpfApp1.Data;
using System.Windows.Controls.Primitives;
using System.Xml.Linq;
using System.Windows.Media;
using System.ComponentModel;
using System.Timers;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace WpfApp1.ViewModels
{
    public class ReversiViewModel : ViewModelBase
    {
        private Logic _logic;
        private int _rowCount;
        //private int _columnCount;
        public ObservableCollection<ColorFieldViewModel> Fields { get; private set; }
        public event EventHandler<EventArgs>? MessageBoxNemSikerult;
        public event EventHandler<EventArgs>? MessageBoxSikerult;
        public event EventHandler<EventArgs>? MessageBoxPause;
        
        public DelegateCommand MyButtonClickCommand { get; }
        public DelegateCommand SaveClickCommand { get; }
        public DelegateCommand PauseCommand { get; }
        public DelegateCommand LoadClickCommand { get; }
        public event EventHandler<EventArgs>? TimerStopped;
        public event EventHandler<EventArgs>? TimerStarted;
        public DelegateCommand ChangeSizeCommand { get; private set; }


        public ReversiViewModel(Logic model)
        {
            _logic = model;
            Fields = new ObservableCollection<ColorFieldViewModel>();
            MyButtonClickCommand = new DelegateCommand((param) =>
            {
                MyButtonClick();
            });
            SaveClickCommand = new DelegateCommand(async (param) =>
            {
                await Save();
            });
            LoadClickCommand = new DelegateCommand(async (param) =>
            {
                await Load();
            });
            PauseCommand = new DelegateCommand((param) =>
            {
                Pause();
            });
            ChangeSizeCommand = new DelegateCommand(x => GenerateFields());
            _logic.TimeAdvanced += new EventHandler<EventArgs>(Model_TimeAdvanced);
        }
        public void StartTimer()
        {
            TimerStarted?.Invoke(this, EventArgs.Empty);
        }
        public void StopTimer()
        {
            TimerStopped?.Invoke(this, EventArgs.Empty);
        }
        public void Model_TimeAdvanced(object? sender, EventArgs e)
        {
            if(_logic.MyData.GetNext() == Next.Black)
            {
                BlackSecs++;
            }
            else
            {
                WhiteSecs++;
            }
        }
        public Int32 RowCount
        {
            get { return _rowCount; }
            set
            {
                if (_rowCount != value)
                {
                    ColumnCount = value;
                    _rowCount = value;
                    //OnPropertyChanged();
                    //ha itt lenne az onPropChanged(), akkor ha elkezdenénk egy új méretet beírni, mielőtt megnyomnánk a méretváltó gombot, váltana a méret
                }
            }
        }

        /// <summary>
        /// Oszlopok számának lekérdezése, vagy beállítása.
        /// </summary>
        public Int32 ColumnCount
        {
            get { return _rowCount; }
            set
            {
                if (_rowCount != value)
                {
                    _rowCount = value;
                    //OnPropertyChanged();
                    //ha itt lenne az onPropChanged(), akkor ha elkezdenénk egy új méretet beírni, mielőtt megnyomnánk a méretváltó gombot, váltana a méret
                }
            }
        }
        public Next MyNext
        {
            get { return _logic.MyData.GetNext(); }
            set { if (_logic.MyData.GetNext() != value)
                {
                    _logic.MyData.SetNext(value);
                    OnPropertyChanged();
                }}
        }
        public int WhiteSecs
        {
            get { return _logic.Whitesecs; }
            set 
            { 
                if(_logic.Whitesecs != value)
                {
                    _logic.Whitesecs = value;
                    OnPropertyChanged();
                } 
            }
        }

        public int BlackSecs
        {
            get { return _logic.Blacksecs; }
            set
            {
                if (_logic.Blacksecs != value)
                {
                    _logic.Blacksecs = value;
                    OnPropertyChanged(nameof(BlackSecs));
                }
            }
        }
        private void GenerateFields()
        {
            StartTimer();
            Fields.Clear();
            OnPropertyChanged(nameof(RowCount));
            OnPropertyChanged(nameof(ColumnCount));
            WhiteSecs = 0;
            BlackSecs = 0;
            _logic.MyData.WhiteSecs = 0;
            _logic.MyData.BlackSecs = 0;
            _logic.MyData.SetTableSize(RowCount);
            _logic.InitTableData();
            for (Int32 rowIndex = 0; rowIndex < RowCount; rowIndex++)
                for (Int32 columnIndex = 0; columnIndex < ColumnCount; columnIndex++)
                {
                    double halfsize = Math.Ceiling(RowCount / 2.0);

                    if ((rowIndex == (halfsize - 1) && columnIndex == (halfsize - 1)) || (rowIndex == halfsize && columnIndex == halfsize))
                    {
                        //Black
                        Fields.Add(new ColorFieldViewModel // mező létrehozása
                        {
                            ColorNumber = 2, // megadjuk a kezdőértékeket
                            Row = rowIndex,
                            Column = columnIndex,
                            FieldChangeCommand = new DelegateCommand(field =>
                            {
                                if (field is ColorFieldViewModel viewModel)
                                    FieldChange(viewModel);
                            }) // és a végrehajtandó parancsot
                        });
                    }
                    else if ((rowIndex == halfsize && columnIndex == (halfsize - 1)) || (rowIndex == (halfsize - 1) && columnIndex == halfsize))
                    {
                        //White
                        Fields.Add(new ColorFieldViewModel // mező létrehozása
                        {
                            ColorNumber = 1, // megadjuk a kezdőértékeket
                            Row = rowIndex,
                            Column = columnIndex,
                            FieldChangeCommand = new DelegateCommand(field =>
                            {
                                if (field is ColorFieldViewModel viewModel)
                                    FieldChange(viewModel);
                            }) // és a végrehajtandó parancsot
                        });
                    }
                    else if ((rowIndex == halfsize && columnIndex == (halfsize - 2)) || (rowIndex == (halfsize - 1) && columnIndex == halfsize + 1) || (rowIndex == (halfsize - 2) && columnIndex == halfsize) || (rowIndex == halfsize + 1 && columnIndex == (halfsize - 1)))
                    {
                        //Pink
                        Fields.Add(new ColorFieldViewModel // mező létrehozása
                        {
                            ColorNumber = 3, // megadjuk a kezdőértékeket
                            Row = rowIndex,
                            Column = columnIndex,
                            FieldChangeCommand = new DelegateCommand(field =>
                            {
                                if (field is ColorFieldViewModel viewModel)
                                    FieldChange(viewModel);
                            }) // és a végrehajtandó parancsot
                        });
                    }
                    else
                    {
                        //Üres
                        Fields.Add(new ColorFieldViewModel // mező létrehozása
                        {
                            ColorNumber = 0, // megadjuk a kezdőértékeket
                            Row = rowIndex,
                            Column = columnIndex,
                            FieldChangeCommand = new DelegateCommand(field =>
                            {
                                if (field is ColorFieldViewModel viewModel)
                                    FieldChange(viewModel);
                            }) // és a végrehajtandó parancsot
                        });
                    }

                }
            StartTimer();
            //OnPropertyChanged(nameof(RowDefinitions));
            //OnPropertyChanged(nameof(ColumnDefinitions));
            /*
            //TimerStop();
            Fields.Clear();
            for (Int32 rowIndex = 0; rowIndex < RowCount; rowIndex++)
                for (Int32 columnIndex = 0; columnIndex < ColumnCount; columnIndex++)
                {
                    if()
                    Fields.Add(new ColorFieldViewModel // mező létrehozása
                    {
                        ColorNumber = 0, // megadjuk a kezdőértékeket
                        Row = rowIndex,
                        Column = columnIndex,
                        FieldChangeCommand = new DelegateCommand(field =>
                        {
                            if (field is ColorFieldViewModel viewModel)
                                FieldChange(viewModel);
                        }) // és a végrehajtandó parancsot
                    });
                }*/

        }
        private void FieldChange(ColorFieldViewModel selectedField)
        {
            /*
            model.istie = false;
            model.whitewon = false;
            model.blackwon = false;
            model.bothpassed = false;
            Message = "Good luck!";
            */
            //OnPropertyChanged(nameof(Message));
            if (_logic.isValidString(selectedField.Row, selectedField.Column, _logic.MyData.GetNext()) != "valid")
            {

            }
            else
            {
                ButtonType actType;
                int graynumber = 0;
                int whitenumber = 1;
                int blacknumber = 2;
                int pinknumber = 3;

                _logic.MakeMove(selectedField.Row, selectedField.Column);
                /*
                if (model.bothpassed) //mindketten passzoltak
                {
                    if (model.istie)
                    {
                        MessageBox.Show("It's a tie!");
                    }
                    else if (model.blackwon)
                    {
                        MessageBox.Show("You both passed! Black won:)");
                    }
                    else if (model.whitewon)
                    {
                        MessageBox.Show("You both passed! White won!");
                    }
                }
                else //betelt a tábla
                {
                    if (model.istie)
                    {
                        MessageBox.Show("Board is full. It's a tie!");
                    }
                    else if (model.blackwon)
                    {
                        MessageBox.Show("Board is full, black won!");
                    }
                    else if (model.whitewon)
                    {
                        MessageBox.Show("Board is full! White won!");
                    }
                }*/

                foreach (ColorFieldViewModel field in Fields)
                {
                    int i = field.Row;
                    int j = field.Column;
                    actType = _logic.MyData.GetData()[i, j].GetButtonType();

                    if (actType == ButtonType.White)
                    {
                        field.ColorNumber = whitenumber;
                    }
                    else if (actType == ButtonType.Black)
                    {
                        field.ColorNumber = blacknumber;
                    }
                    else if (actType == ButtonType.Candidate)
                    {
                        field.ColorNumber = pinknumber;
                    }
                    else
                    {
                        field.ColorNumber = graynumber;
                    }
                }

            }
            /*//TimerStop();
            //Int32 color = (selectedField.ColorNumber + 1) % 3;
            // a rákövetkező színt vesszük
            int i = selectedField.Row;
            int j = selectedField.Column;
            if()

            foreach (ColorFieldViewModel field in Fields)
            {
                if (field.Column == selectedField.Column || field.Row == selectedField.Row) // adott oszlopban és sorban
                    field.ColorNumber = color; // átszínezés végrehajtása
            }*/
        }
        private void Pause()
        {
            StopTimer();
            //var r = MessageBox.Show("Game Paused!");
            MessageBoxPause?.Invoke(this, EventArgs.Empty);
            StartTimer();
        }

        private async Task Save()
        {
            try
            {
                await _logic.SaveGameAsync("reversi");
                //MessageBox.Show("siker:)");
                MessageBoxSikerult?.Invoke(this, EventArgs.Empty);
            }
            catch
            {
                //MessageBox.Show("nem sikerült menteni a játékot");
                MessageBoxNemSikerult?.Invoke(this, EventArgs.Empty);
            }
        }
        private async Task Load()
        {
            Fields.Clear();
            try
            {
                await _logic.LoadGameAsync("reversi");
                if(_logic.MyData.GetTableSize() == 10)
                {
                    RowCount = 10;
                    ColumnCount = 10;
                    OnPropertyChanged(nameof(RowCount));
                    OnPropertyChanged(nameof(ColumnCount));
                    //GenerateFields();
                }else if(_logic.MyData.GetTableSize() == 20)
                {
                    RowCount = 20;
                    ColumnCount = 20;
                    OnPropertyChanged(nameof(RowCount));
                    OnPropertyChanged(nameof(ColumnCount));
                    //GenerateFields();
                }
                else
                {
                    RowCount = 30;
                    ColumnCount = 30;
                    OnPropertyChanged(nameof(RowCount));
                    OnPropertyChanged(nameof(ColumnCount));
                    //GenerateFields();
                }
                
                WhiteSecs = _logic.Whitesecs;
                BlackSecs = _logic.Blacksecs;

                //uj
                for (Int32 rowIndex = 0; rowIndex < RowCount; rowIndex++)
                    for (Int32 columnIndex = 0; columnIndex < ColumnCount; columnIndex++)
                    {
                        double halfsize = Math.Ceiling(RowCount / 2.0);

                        if ((rowIndex == (halfsize - 1) && columnIndex == (halfsize - 1)) || (rowIndex == halfsize && columnIndex == halfsize))
                        {
                            //Black
                            Fields.Add(new ColorFieldViewModel // mező létrehozása
                            {
                                ColorNumber = 2, // megadjuk a kezdőértékeket
                                Row = rowIndex,
                                Column = columnIndex,
                                FieldChangeCommand = new DelegateCommand(field =>
                                {
                                    if (field is ColorFieldViewModel viewModel)
                                        FieldChange(viewModel);
                                }) // és a végrehajtandó parancsot
                            });
                        }
                        else if ((rowIndex == halfsize && columnIndex == (halfsize - 1)) || (rowIndex == (halfsize - 1) && columnIndex == halfsize))
                        {
                            //White
                            Fields.Add(new ColorFieldViewModel // mező létrehozása
                            {
                                ColorNumber = 1, // megadjuk a kezdőértékeket
                                Row = rowIndex,
                                Column = columnIndex,
                                FieldChangeCommand = new DelegateCommand(field =>
                                {
                                    if (field is ColorFieldViewModel viewModel)
                                        FieldChange(viewModel);
                                }) // és a végrehajtandó parancsot
                            });
                        }
                        else if ((rowIndex == halfsize && columnIndex == (halfsize - 2)) || (rowIndex == (halfsize - 1) && columnIndex == halfsize + 1) || (rowIndex == (halfsize - 2) && columnIndex == halfsize) || (rowIndex == halfsize + 1 && columnIndex == (halfsize - 1)))
                        {
                            //Pink
                            Fields.Add(new ColorFieldViewModel // mező létrehozása
                            {
                                ColorNumber = 3, // megadjuk a kezdőértékeket
                                Row = rowIndex,
                                Column = columnIndex,
                                FieldChangeCommand = new DelegateCommand(field =>
                                {
                                    if (field is ColorFieldViewModel viewModel)
                                        FieldChange(viewModel);
                                }) // és a végrehajtandó parancsot
                            });
                        }
                        else
                        {
                            //Üres
                            Fields.Add(new ColorFieldViewModel // mező létrehozása
                            {
                                ColorNumber = 0, // megadjuk a kezdőértékeket
                                Row = rowIndex,
                                Column = columnIndex,
                                FieldChangeCommand = new DelegateCommand(field =>
                                {
                                    if (field is ColorFieldViewModel viewModel)
                                        FieldChange(viewModel);
                                }) // és a végrehajtandó parancsot
                            });
                        }
                    }
                        //uj vege


                        //MessageBox.Show("siker:)");
                        MessageBoxSikerult?.Invoke(this, EventArgs.Empty);
                foreach(ColorFieldViewModel field in Fields)
                {
                    int i = (int)field.Row;
                    int j = (int)field.Column;
                    if (_logic.MyData.GetData()[i, j].GetButtonType() == ButtonType.White)
                    {
                        field.ColorNumber = 1;
                    }else if(_logic.MyData.GetData()[i, j].GetButtonType() == ButtonType.Black)
                    {
                        field.ColorNumber = 2;
                    }else if(_logic.MyData.GetData()[i, j].GetButtonType() == ButtonType.Empty)
                    {
                        field.ColorNumber = 0;
                    }
                    else
                    {
                        field.ColorNumber = 3;
                    }
                }
            }catch
            {
                //MessageBox.Show("nem sikerült betölteni a játékot");
                MessageBoxNemSikerult?.Invoke(this, EventArgs.Empty);
            }
        }
        private void MyButtonClick()
        {
            
            /*
            if (parameter is string buttonName)
            {
                buttonName = buttonName.Remove(0, 1);
                string[] string_stripped = buttonName.Split('_');
                int x = int.Parse(string_stripped[0]);
                int y = int.Parse(string_stripped[1]);
                string jo = _logic.isValidString(x, y, _logic.MyData.GetNext());
                if (jo == "valid")
                {
                    _logic.MakeMove(x, y);
                }
                MessageBox.Show($"Button {buttonName} Clicked! {jo}");

                SolidColorBrush newColor = new SolidColorBrush(Colors.Black);

                // Assign the new SolidColorBrush to the button's Background property
                OnPropertyChanged(nameof(BlackSecs));
            }

            /*if (parameter is string buttonName)
            {
                buttonName = buttonName.Remove(0, 1);
                string[] string_stripped = buttonName.Split('_');
                int x = int.Parse(string_stripped[0]);
                int y = int.Parse(string_stripped[1]);
                string jo = _logic.isValidString(x, y, _logic.MyData.GetNext());
                if (jo == "valid")
                {
                    OnPropertyChanged(nameof(MyButtonClick));
                }

                MessageBox.Show($"Button {buttonName} Clicked! {jo}");
                
            }*/
        }

    }

}
