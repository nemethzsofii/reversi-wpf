using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Data
{
    public enum Next
    {
        White,
        Black
    }
    public enum ButtonType
    {
        White,
        Black,
        Empty,
        Candidate
    }

    public class ButtonData
    {
        private ButtonType buttonType;
        private int xPos;
        private int yPos;
        public ButtonType GetButtonType()
        {
            return buttonType;
        }
        public int getXPos()
        {
            return xPos;
        }
        public int getYPos()
        {
            return yPos;
        }
        public void SetButtonType(ButtonType buttonType)
        {
            this.buttonType = buttonType;
        }
        public void SetXPos(int xPos)
        {
            this.xPos = xPos;
        }
        public void SetYPos(int yPos)
        {
            this.yPos = yPos;
        }
    }
    public class WpfData
    {
        private int tableSize = 10;
        private ButtonData[,] data = new ButtonData[10, 10];
        public event EventHandler? TableChanged;
        private Next next = Next.Black;

        public int WhiteSecs { get; set; }
        public int BlackSecs { get; set; }

        //private int _tableSize;

        public WpfData()
        {
            WhiteSecs = 0;
            BlackSecs = 0;
        }

        public int GetTableSize()
        {
            return tableSize;
        }

        public Next GetNext()
        {
            return next;
        }

        public ButtonData[,] GetData()
        {
            return data;
        }
        public void SetTableSize(int s)
        {
            tableSize = s;
            data = new ButtonData[s, s];
        }

        public void SetNext(Next x)
        {
            next = x;
        }

        public void SetData(ButtonData[,] d)
        {
            data = d;
            TableChanged?.Invoke(this, EventArgs.Empty);
        }
        public void SetOneData(int r, int c, ButtonData d)
        {
            data[r, c] = d;
            TableChanged?.Invoke(this, EventArgs.Empty);
        }

        public ButtonData GetTableData(int xPos, int yPos)
        {
            //return _myData.data[xPos, yPos];
            return GetData()[xPos, yPos];
        }

        public static int ButtonTypeToInt(ButtonType b)
        {
            switch (b)
            {
                case ButtonType.Empty:
                    return 0;
                case ButtonType.White:
                    return 1;
                case ButtonType.Black:
                    return 2;
                default:
                    return 3;
            }
        }
        public static ButtonType IntToButtonType(int i)
        {
            switch (i)
            {
                case 0:
                    return ButtonType.Empty;
                case 1:
                    return ButtonType.White;
                case 2:
                    return ButtonType.Black;
                default:
                    return ButtonType.Candidate;
            }
        }

        public void SetTableData(ButtonType type, int xPos, int yPos)
        {
            ButtonData button = new ButtonData();
            button.SetButtonType(type);
            button.SetXPos(xPos);
            button.SetYPos(yPos);
            //_myData.data[xPos, yPos] = button;
            SetOneData(xPos, yPos, button);
            TableChanged?.Invoke(this, EventArgs.Empty);
        }
    }

}
