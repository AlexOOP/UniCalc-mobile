using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UniCalcMob.Models;
using Xamarin.Forms;
using System.Text.RegularExpressions;

namespace UniCalcMob.Views
{
  public class CalcPage : ContentPage
  {
    private Picker _picker;
    private Button _button;
    private Entry _entryHiLow;
    private Entry _entryHex;
    private Entry _entryHDLC;
    private Entry _entrySerial;
    private Entry _entryField1;
    private Entry _entryField2;

    int step = 16384;
    int startHi = 16;
    int endHi = 16383;
    int startLow = 16;
    int endLow = 16383;
    int inputHi, inputLow;
    string pluralSerialShow;
    List<int> SerialList = new List<int>();
    ushort HdlcHi = 0;
    ushort HdlcLow = 1;
    public int soughtSerial;
    FixedValues fixedValues = new FixedValues();

    int _ser, _hi, _low, _hexHi, _hexLow, _hdlcHi, _hdlcLow;

    public CalcPage()
    {
      this.Title = "Calculation Page";

      List<KnownValue> knownValues = new List<KnownValue>();
      knownValues.Add(new KnownValue() { Id = 1, Name = "Считать из SN" });
      knownValues.Add(new KnownValue() { Id = 2, Name = "Считать из HI LOW" });
      knownValues.Add(new KnownValue() { Id = 3, Name = "Считать из HEX" });
      knownValues.Add(new KnownValue() { Id = 4, Name = "Считать из HDLC" });

      StackLayout stackLayout = new StackLayout();
      _picker = new Picker();

      _picker.Title = "Выберите известное значение";
      _picker.ItemsSource = knownValues;
      _picker.TextColor = Color.FromHex("#596f92");
      stackLayout.Children.Add(_picker);

      _entryField1 = new Entry();
      _entryField1.BackgroundColor = Color.FromHex("#202124");
      _entryField1.TextColor = Color.FromHex("eeeeee");
      _entryField1.PlaceholderColor = Color.FromHex("#596f92");
      _entryField1.Keyboard = Keyboard.Numeric;
      //_entryField1.HorizontalTextAlignment = TextAlignment.Center;
      if (_picker.SelectedIndex == 0)
        _entryField1.Placeholder = "Введите серийный номер";
      else _entryField1.Placeholder = "Введите значение";
      stackLayout.Children.Add(_entryField1);
      _entryField1.TextChanged += _entryField1_TextChanged;

      _entryField2 = new Entry();
      _entryField2.BackgroundColor = Color.FromHex("#202124");
      _entryField2.TextColor = Color.FromHex("eeeeee");
      _entryField2.PlaceholderColor = Color.FromHex("#596f92");
      _entryField2.Keyboard = Keyboard.Numeric;
      _entryField2.IsVisible = false;
      //_entryField2.HorizontalTextAlignment = TextAlignment.Center;
      _entryField2.Placeholder = "Введите значение";
      stackLayout.Children.Add(_entryField2);
      _entryField2.TextChanged += _entryField2_TextChanged;

      _button = new Button();
      _button.BackgroundColor = Color.FromHex("#2c3e50");
      _button.TextColor = Color.White;
      _button.Text = "Рассчитать";
      _button.Clicked += _button_Clicked;
      stackLayout.Children.Add(_button);

      _entrySerial = new Entry();
      _entrySerial.Keyboard = Keyboard.Text;
      //_entrySerial.HorizontalTextAlignment = TextAlignment.Center;
      _entrySerial.Placeholder = "SN";
      _entrySerial.BackgroundColor = Color.FromHex("#202124");
      _entrySerial.PlaceholderColor = Color.White;
      _entrySerial.TextColor = Color.White;
      //_entrySerial.IsEnabled = false;
      stackLayout.Children.Add(_entrySerial);
      _entrySerial.IsVisible = false;
      _entrySerial.InputTransparent = true;

      _entryHiLow = new Entry();
      _entryHiLow.Keyboard = Keyboard.Text;
      //_entryHiLow.HorizontalTextAlignment = TextAlignment.Center;
      _entryHiLow.Placeholder = "HI LOW";
      _entryHiLow.BackgroundColor = Color.FromHex("#202124");
      _entryHiLow.PlaceholderColor = Color.White;
      _entryHiLow.TextColor = Color.White;
      //_entryHiLow.IsEnabled = false;
      stackLayout.Children.Add(_entryHiLow);
      _entryHiLow.InputTransparent = true;

      _entryHex = new Entry();
      _entryHex.Keyboard = Keyboard.Text;
      //_entryHex.HorizontalTextAlignment = TextAlignment.Center;
      _entryHex.Placeholder = "HEX";
      _entryHex.BackgroundColor = Color.FromHex("#202124");
      _entryHex.PlaceholderColor = Color.White;
      _entryHex.TextColor = Color.White;
      //_entryHex.IsEnabled = false;
      stackLayout.Children.Add(_entryHex);
      _entryHex.InputTransparent = true;

      _entryHDLC = new Entry();
      _entryHDLC.Keyboard = Keyboard.Text;
      //_entryHDLC.HorizontalTextAlignment = TextAlignment.Center;
      _entryHDLC.Placeholder = "HDLC";
      _entryHDLC.BackgroundColor = Color.FromHex("#202124");
      _entryHDLC.PlaceholderColor = Color.White;
      _entryHDLC.TextColor = Color.White;
      //_entryHDLC.IsEnabled = false;
      stackLayout.Children.Add(_entryHDLC);
      _entryHDLC.InputTransparent = true;

      //Правило отображения второго поля для ввода
      _picker.SelectedIndex = 0;
      _picker.SelectedIndexChanged += _picker_SelectedIndexChanged;

      stackLayout.BackgroundColor = Color.FromHex("#323639");
      Content = stackLayout;
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // регулярные выражения для ввода только Dec и Hex значений
    private void _entryField1_TextChanged(object sender, TextChangedEventArgs e)
    {
      if (!Regex.IsMatch(e.NewTextValue, "^[0-9a-fA-F]+$", RegexOptions.CultureInvariant))
      {
        (sender as Entry).Text = Regex.Replace(e.NewTextValue, "[^0-9a-fA-F]", string.Empty);
      }
    }

    private void _entryField2_TextChanged(object sender, TextChangedEventArgs e)
    {
      if (!Regex.IsMatch(e.NewTextValue, "^[0-9a-fA-F]+$", RegexOptions.CultureInvariant))
      {
        (sender as Entry).Text = Regex.Replace(e.NewTextValue, "[^0-9a-fA-F]", string.Empty);
      }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //определение типа клавиатуры при переключении известного значения
    private void _picker_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (_picker.SelectedIndex == 0)
      {
        _entryField1.Keyboard = Keyboard.Numeric;
        _entryField2.Keyboard = Keyboard.Numeric;
        _entrySerial.IsVisible = false;
        _entryHiLow.IsVisible = true;
        _entryHex.IsVisible = true;
        _entryHDLC.IsVisible = true;
      }
      else if (_picker.SelectedIndex == 1)
      {
        _entryField1.Keyboard = Keyboard.Numeric;
        _entryField2.Keyboard = Keyboard.Numeric;
        _entrySerial.IsVisible = false;
        _entryHiLow.IsVisible = false;
        _entryHex.IsVisible = true;
        _entryHDLC.IsVisible = true;
      }
      else if (_picker.SelectedIndex == 2)
      {
        _entryField1.Keyboard = Keyboard.Text;
        _entryField2.Keyboard = Keyboard.Text;
        _entrySerial.IsVisible = false;
        _entryHiLow.IsVisible = true;
        _entryHex.IsVisible = false;
        _entryHDLC.IsVisible = true;
      }
      else if (_picker.SelectedIndex == 3)
      {
        _entryField1.Keyboard = Keyboard.Text;
        _entryField2.Keyboard = Keyboard.Text;
        _entrySerial.IsVisible = false;
        _entryHiLow.IsVisible = true;
        _entryHex.IsVisible = true;
        _entryHDLC.IsVisible = false;
      }
      if (_picker.SelectedIndex == 0 || _picker.IsFocused == false)
      {
        _entryField2.IsVisible = false;
      }
      else
      {
        _entryField2.IsVisible = true;
      }
      _entryField1.Text = "";
      _entryField2.Text = "";
      ClearAllFields();
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //Логика по нажатию кнопки Рассчитать
    private void _button_Clicked(object sender, EventArgs e)
    {
      ClearAllFields();
      // событие, по которому нужно расчитывать Hi Low/HEX/HDLC из серийника
      if (_picker.SelectedIndex == 0)
      {
        if ((String.IsNullOrWhiteSpace(_entryField1.Text)) || (Convert.ToDouble(_entryField1.Text) > 2147483647) || (Convert.ToDouble(_entryField1.Text) < 1))
        {
          DisplayAlert("Предупреждение:", "Введите число от 16 до 2147483647!", "OK");
          ClearAllFields();
          return;
        }
        else
        {
          Serial2HiLow();
        }

      }
      // событие, по которому нужно расчитывать серийник/HEX/HDLC из Hi Low
      if (_picker.SelectedIndex == 1)
      {
        if ((String.IsNullOrWhiteSpace(_entryField1.Text)) || (Convert.ToDouble(_entryField1.Text) > 16383) || (Convert.ToDouble(_entryField1.Text) < 16) || 
          (String.IsNullOrWhiteSpace(_entryField2.Text)) || (Convert.ToDouble(_entryField2.Text) > 16383) || (Convert.ToDouble(_entryField2.Text) < 16))
        {
          DisplayAlert("Предупреждение:", "Введите число от 16 до 16383!", "OK");
          ClearAllFields();
          return;
        }
        else
        {
          HiLow2SN();
        }
      }
      // событие, по которому нужно расчитывать серийник/Hi Low/HDLC из HEX
      if (_picker.SelectedIndex == 2)
      {
        if ((String.IsNullOrWhiteSpace(_entryField1.Text)) || (Convert.ToDouble(Convert.ToString(Convert.ToInt32(_entryField1.Text, 16))) > 16383) || 
          (Convert.ToDouble(Convert.ToString(Convert.ToInt32(_entryField1.Text, 16))) < 16) || (String.IsNullOrWhiteSpace(_entryField2.Text)) || 
          (Convert.ToDouble(Convert.ToString(Convert.ToInt32(_entryField2.Text, 16))) > 16383) || (Convert.ToDouble(Convert.ToString(Convert.ToInt32(_entryField2.Text, 16))) < 16))
        {
          DisplayAlert("Предупреждение:", "Введите число от 16 до 16383!", "OK");
          ClearAllFields();
          return;
        }
        else
        {
          Hex2DecShow();
          Hex2Hdlc();
          HiLow2SN();
        }
      }
      // событие, по которому нужно расчитывать серийник/Hi Low/HEX из HDLC
      if (_picker.SelectedIndex == 3)
      {
        if ((String.IsNullOrWhiteSpace(_entryField1.Text)) || (Convert.ToDouble(Convert.ToString(Convert.ToInt32(_entryField1.Text, 16))) > 65278) || 
          (Convert.ToDouble(Convert.ToString(Convert.ToInt32(_entryField1.Text, 16))) < 20) || (String.IsNullOrWhiteSpace(_entryField2.Text)) || 
          (Convert.ToDouble(Convert.ToString(Convert.ToInt32(_entryField2.Text, 16))) > 65279) || (Convert.ToDouble(Convert.ToString(Convert.ToInt32(_entryField2.Text, 16))) < 21))
        {
          DisplayAlert("Предупреждение:", "Введите число от 16 до 16383!", "OK");
          ClearAllFields();
          return;
        }
        else
        {
          Hdlc2Hex();
          Hex2DecShow();
          HiLow2SN();
        }
      }
    }

    //Очистка всех полей вывода
    private void ClearAllFields()
    {
      _entryHiLow.Text = "";
      _entryHex.Text = "";
      _entryHDLC.Text = "";
      _entrySerial.Text = "";
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //алгоритм расчёта hi low из серийного номера

    private void Serial2HiLow()
    {
      if (_picker.SelectedIndex == 0)
      {
        _ser = int.Parse(_entryField1.Text);

        ulong HI = 0;
        ulong LOW = 0;
        ulong short_serial = (ulong)_ser;

        bool isNewAlgorithm = true;
        if ((short_serial & 0x1000) == 0)
        {
          if (short_serial == 0x2000)
          {
            isNewAlgorithm = false;
          }
        }

        // доработанный алгоритм расчёта определённых диапазонов Hi Low из серийников
        if (short_serial == 0x2000)
        {
          isNewAlgorithm = false;
        }

        if (!isNewAlgorithm)
        {
          HI = (short_serial & 0x3FFF) < 16 ? short_serial = (1 << 13) : short_serial & 0x3FFF;
          LOW = ((short_serial >> 14) & 0x3FFF) < 16 ? (1 << 13) : (short_serial >> 14) & 0x3FFF;
        }
        else
        {
          HI = 0;
          LOW = short_serial & 0x3FFF;

          if ((short_serial & 0x3FFF) < 16)
          {
            LOW += 16;
            HI |= (1 << 12);
          }
          else
          {
            LOW = short_serial & 0x3FFF;
          }

          if (((short_serial >> 14) & 0x3FFF) < 16)
          {
            HI += ((short_serial >> 14) & 0x3FFF) + 16;
            HI |= (1 << 13);
          }
          else
          {
            HI |= (short_serial >> 14) & 0x3FFF;
          }
        }

        _hi = (int)HI;
        _low = (int)LOW;
        _entryHiLow.Text = _hi.ToString() + "-" + _low.ToString();

        _hexHi = (int)HI;
        _hexLow = (int)LOW;
        _entryHex.Text = Convert.ToString(_hexHi, 16) + "-" + Convert.ToString(_hexLow, 16);
        Hex2Hdlc();
      }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // алгоритм расчёта HDLC из HEX
    public int Hdlc_HiLow(int _a, int _l)
    {
      int result = (((((_a) << 1) & 0x00FE) | (_l)) | (((_a) << 2) & 0xFE00));
      return result;
    }

    // метод расчёта HDLC из HEX 
    public void Hex2Hdlc()
    {
      if (_picker.SelectedIndex == 2)
      {
        int inHi = Convert.ToInt32(_entryField1.Text, 16);
        int inLow = Convert.ToInt32(_entryField2.Text, 16);


        int inHiRes = Hdlc_HiLow(inHi, HdlcHi);
        int inLowRes = Hdlc_HiLow(inLow, HdlcLow);

        _hdlcHi = inHiRes;
        _hdlcLow = inLowRes;
      }
      else
      {
        string[] a = (_entryHex.Text).Split('-');

        string hiHdlc = Convert.ToString(Convert.ToInt32(a[0], 16));
        string lowHdlc = Convert.ToString(Convert.ToInt32(a[1], 16));

        int inHi = Convert.ToInt32(hiHdlc);
        int inLow = Convert.ToInt32(lowHdlc);

        int inHiRes = Hdlc_HiLow(inHi, HdlcHi);
        int inLowRes = Hdlc_HiLow(inLow, HdlcLow);

        _hdlcHi = inHiRes;
        _hdlcLow = inLowRes;
      }
      _entryHDLC.Text = Convert.ToString(_hdlcHi, 16) + "-" + Convert.ToString(_hdlcLow, 16);
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // метод расчёта всех возможных серийников из Hi Low
    public int SpecialCountSerialFromHiLow(int primaryKeyDict, int firstValueDict)
    {
      return soughtSerial = firstValueDict + ((inputHi - primaryKeyDict) * step + (inputLow - startLow));
    }

    // алгоритм расчёта из HI LOW -> Serial Number
    private void HiLow2SN()
    {
      if (_picker.SelectedIndex == 1)
      {
        inputHi = Math.Abs(Convert.ToInt32(int.Parse(_entryField1.Text)));
        inputLow = Math.Abs(Convert.ToInt32(int.Parse(_entryField2.Text)));
      }
      if (_picker.SelectedIndex == 2 || _picker.SelectedIndex == 3)
      {
        string[] a = (_entryHiLow.Text).Split('-');
        string x = a[0];
        string y = a[1];

        inputHi = Math.Abs(Convert.ToInt32(int.Parse(x)));
        inputLow = Math.Abs(Convert.ToInt32(int.Parse(y)));
      }

      if (inputHi >= 8208 && inputHi <= 8223 && inputLow >= 16 && inputLow <= 16383)
      {
        SpecialCountSerialFromHiLow(fixedValues.primaryKeyDict_20, fixedValues.firstValueDict_20);
        SerialList.Add(soughtSerial);
        SpecialCountSerialFromHiLow(fixedValues.primaryKeyDict_22, fixedValues.firstValueDict_22);
        SerialList.Add(soughtSerial);
        SpecialCountSerialFromHiLow(fixedValues.primaryKeyDict_24, fixedValues.firstValueDict_24);
        SerialList.Add(soughtSerial);
        SpecialCountSerialFromHiLow(fixedValues.primaryKeyDict_26, fixedValues.firstValueDict_26);
        SerialList.Add(soughtSerial);
        SpecialCountSerialFromHiLow(fixedValues.primaryKeyDict_28, fixedValues.firstValueDict_28);
        SerialList.Add(soughtSerial);
      }

      if (inputHi >= 16 && inputHi <= 16383 && inputLow >= 16 && inputLow <= 16383)
      {
        SpecialCountSerialFromHiLow(fixedValues.primaryKeyDict_21, fixedValues.firstValueDict_21);
        SerialList.Add(soughtSerial);
        SpecialCountSerialFromHiLow(fixedValues.primaryKeyDict_23, fixedValues.firstValueDict_23);
        SerialList.Add(soughtSerial);
        SpecialCountSerialFromHiLow(fixedValues.primaryKeyDict_25, fixedValues.firstValueDict_25);
        SerialList.Add(soughtSerial);
        SpecialCountSerialFromHiLow(fixedValues.primaryKeyDict_27, fixedValues.firstValueDict_27);
        SerialList.Add(soughtSerial);
      }

      if (inputHi >= 4096 && inputHi <= 8191 && inputLow >= 16 && inputLow <= 31)
      {
        SpecialCountSerialFromHiLow(fixedValues.primaryKeyDict_3, fixedValues.firstValueDict_3);
        SerialList.Add(soughtSerial);
        SpecialCountSerialFromHiLow(fixedValues.primaryKeyDict_8, fixedValues.firstValueDict_8);
        SerialList.Add(soughtSerial);
        SpecialCountSerialFromHiLow(fixedValues.primaryKeyDict_13, fixedValues.firstValueDict_13);
        SerialList.Add(soughtSerial);
        SpecialCountSerialFromHiLow(fixedValues.primaryKeyDict_18, fixedValues.firstValueDict_18);
        SerialList.Add(soughtSerial);
      }

      if (inputHi >= 4112 && inputHi <= 8191 && inputLow >= 16 && inputLow <= 31)
      {
        SpecialCountSerialFromHiLow(fixedValues.primaryKeyDict_2, fixedValues.firstValueDict_2);
        SerialList.Add(soughtSerial);
        SpecialCountSerialFromHiLow(fixedValues.primaryKeyDict_7, fixedValues.firstValueDict_7);
        SerialList.Add(soughtSerial);
        SpecialCountSerialFromHiLow(fixedValues.primaryKeyDict_12, fixedValues.firstValueDict_12);
        SerialList.Add(soughtSerial);
        SpecialCountSerialFromHiLow(fixedValues.primaryKeyDict_17, fixedValues.firstValueDict_17);
        SerialList.Add(soughtSerial);
      }

      if (inputHi >= 12304 && inputHi <= 12319 && inputLow >= 16 && inputLow <= 31)
      {
        SpecialCountSerialFromHiLow(fixedValues.primaryKeyDict_1, fixedValues.firstValueDict_1);
        SerialList.Add(soughtSerial);
        SpecialCountSerialFromHiLow(fixedValues.primaryKeyDict_6, fixedValues.firstValueDict_6);
        SerialList.Add(soughtSerial);
        SpecialCountSerialFromHiLow(fixedValues.primaryKeyDict_11, fixedValues.firstValueDict_11);
        SerialList.Add(soughtSerial);
        SpecialCountSerialFromHiLow(fixedValues.primaryKeyDict_16, fixedValues.firstValueDict_16);
        SerialList.Add(soughtSerial);
      }

      if (inputHi >= 12288 && inputHi <= 16383 && inputLow >= 16 && inputLow <= 31)
      {
        SpecialCountSerialFromHiLow(fixedValues.primaryKeyDict_4, fixedValues.firstValueDict_4);
        SerialList.Add(soughtSerial);
        SpecialCountSerialFromHiLow(fixedValues.primaryKeyDict_5, fixedValues.firstValueDict_5);
        SerialList.Add(soughtSerial);
        SpecialCountSerialFromHiLow(fixedValues.primaryKeyDict_9, fixedValues.firstValueDict_9);
        SerialList.Add(soughtSerial);
        SpecialCountSerialFromHiLow(fixedValues.primaryKeyDict_10, fixedValues.firstValueDict_10);
        SerialList.Add(soughtSerial);
        SpecialCountSerialFromHiLow(fixedValues.primaryKeyDict_14, fixedValues.firstValueDict_14);
        SerialList.Add(soughtSerial);
        SpecialCountSerialFromHiLow(fixedValues.primaryKeyDict_15, fixedValues.firstValueDict_15);
        SerialList.Add(soughtSerial);
        SpecialCountSerialFromHiLow(fixedValues.primaryKeyDict_19, fixedValues.firstValueDict_19);
        SerialList.Add(soughtSerial);
      }

      SerialList.Sort();

      /*
      if (serialUnder8tsmi.Checked)
      {
        SerialList.RemoveAll(item => item > 99999999);
      }
      else
      {
        SerialList.RemoveAll(item => item > 1074003967);
      }
      */

      StringBuilder builder = new StringBuilder();

      foreach (int item in SerialList)
      {
        builder.Append(item).Append("; ");
      }

      pluralSerialShow = builder.ToString();


      if (SerialList.Count == 1/* && serialUnder8tsmi.Checked*/)
      {
        DisplayAlert("Список возможных серийных номеров до 99 999 999:", pluralSerialShow, "OK");
        SerialList.Clear();
      }
      if (SerialList.Count > 1/* && serialUnder10tsmi.Checked*/)
      {
        DisplayAlert("Список возможных серийных номеров до 1 074 003 967:", pluralSerialShow, "OK");
        SerialList.Clear();
      }
      else
      {
        pluralSerialShow = pluralSerialShow.Replace(";", "");
        _ser = Convert.ToInt32(pluralSerialShow);
        SerialList.Clear();
      }

      if (inputHi > endHi)
      {
        DisplayAlert("Предупреждение:", "Введите число до 16383!", "OK");
        //ClearAllFields();
      }

      if (inputLow > endLow)
      {
        DisplayAlert("Предупреждение:", "Введите число до 16383!", "OK");
        //ClearAllFields();
      }

      // алгоритм расчёта серийных номеров от 262160
      //      _ser = (step * (inputHi - startHi) + startSerNumb + (inputLow - startLow)).ToString();
      _hexHi = inputHi;
      _hexLow = inputLow;
      _entryHex.Text = Convert.ToString(_hexHi, 16) + "-" + Convert.ToString(_hexLow, 16);
      Hex2Hdlc();
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // алгоритм расчёта HEX из HDLC
    public int HiLow_Hdlc(int _a)
    {
      int result = ((((_a) >> 1) & 0x007F) | (((_a) >> 2) & 0x3F80));
      return result;
    }

    // метод расчёта HEX из HDLC
    public void Hdlc2Hex()
    {
      int inHi = Convert.ToInt32(_entryField1.Text, 16);
      int inLow = Convert.ToInt32(_entryField2.Text, 16);

      int inHiRes = HiLow_Hdlc(inHi);
      int inLowRes = HiLow_Hdlc(inLow);

      _hexHi = inHiRes;
      _hexLow = inLowRes;
      _entryHex.Text = Convert.ToString(_hexHi, 16) + "-" + Convert.ToString(_hexLow, 16);
    }


    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // алгоритм расчёта HEX -> HI LOW
    private int HexToDec(string hex)
    {
      int dec = 0;
      for (int i = 0, j = hex.Length - 1; i < hex.Length; i++, j--)
      {
        if (hex[i] == 'A' || hex[i] == 'a') { dec += 10 * (int)Math.Pow(16, j); }
        else if (hex[i] == 'B' || hex[i] == 'b') { dec += 11 * (int)Math.Pow(16, j); }
        else if (hex[i] == 'C' || hex[i] == 'c') { dec += 12 * (int)Math.Pow(16, j); }
        else if (hex[i] == 'D' || hex[i] == 'd') { dec += 13 * (int)Math.Pow(16, j); }
        else if (hex[i] == 'E' || hex[i] == 'e') { dec += 14 * (int)Math.Pow(16, j); }
        else if (hex[i] == 'F' || hex[i] == 'f') { dec += 15 * (int)Math.Pow(16, j); }
        else { dec += (hex[i] - '0') * (int)Math.Pow(16, j); }
      }
      return dec;
    }

    // метод расчёта HEX -> HI LOW
    public void Hex2DecShow()
    {
      // расчёт HDLC -> HI LOW (через HEX)
      if (_picker.SelectedIndex == 2)
      {
        string hiHex = Convert.ToString(Convert.ToInt32(_entryField1.Text, 16));
        string lowHex = Convert.ToString(Convert.ToInt32(_entryField2.Text, 16));

        _hi = HexToDec(hiHex);
        _low = HexToDec(lowHex);
      }
      else
      {
        string[] a = (_entryHex.Text).Split('-');
        string x = a[0];
        string y = a[1];

        string hiHex = Convert.ToString(Convert.ToInt32(x, 16));
        string lowHex = Convert.ToString(Convert.ToInt32(y, 16));

        _hi = HexToDec(hiHex);
        _low = HexToDec(lowHex);
      }

      _entryHiLow.Text = Convert.ToString(_hi, 16) + "-" + Convert.ToString(_low, 16);
    }
  }
}