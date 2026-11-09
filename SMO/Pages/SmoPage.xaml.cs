using Microsoft.Win32;
using OfficeOpenXml;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Vichislenia;

namespace SMO.Pages
{
    public partial class SmoPage : Page
    {
        private readonly SmoType _type;
        private SmoInput? _lastInput;
        private SmoResult? _lastResult;

        public SmoPage(SmoType type)
        {
            _type = type;
            InitializeComponent();
            Configure();
        }

        private void Configure()
        {
            TitleText.Text = TypeName(_type);

            bool isMulti = _type is SmoType.MultiRefusals
                or SmoType.MultiLimitedQueue
                or SmoType.MultiUnlimitedQueue;
            bool hasQueue = _type is SmoType.SingleLimitedQueue
                or SmoType.MultiLimitedQueue;

            ChannelsRow.Visibility = isMulti ? Visibility.Visible : Visibility.Collapsed;
            QueueRow.Visibility = hasQueue ? Visibility.Visible : Visibility.Collapsed;

            ChannelsBox.Text = "2";
            QueueBox.Text = "3";
        }

        private void Calculate_Click(object sender, RoutedEventArgs e)
        {
            ErrorText.Text = "";
            StepsText.Text = "";
            ResultsText.Text = "";

            if (!TryParseDouble(LambdaBox.Text, out double lambda))
            {
                ErrorText.Text = "Некорректное значение λ.";
                return;
            }

            bool useMu = MuRadio.IsChecked == true;
            if (!TryParseDouble(ServiceBox.Text, out double serviceValue))
            {
                ErrorText.Text = useMu ? "Некорректное значение μ." : "Некорректное значение t_обс.";
                return;
            }

            int channels = 1;
            if (ChannelsRow.Visibility == Visibility.Visible
                && !int.TryParse(ChannelsBox.Text?.Trim(), out channels))
            {
                ErrorText.Text = "Некорректное число каналов n.";
                return;
            }

            int queueLength = 0;
            if (QueueRow.Visibility == Visibility.Visible
                && !int.TryParse(QueueBox.Text?.Trim(), out queueLength))
            {
                ErrorText.Text = "Некорректная длина очереди m.";
                return;
            }

            TimeUnit lambdaUnit = SelectedUnit(LambdaUnitBox);
            TimeUnit serviceUnit = SelectedUnit(ServiceUnitBox);

            var input = new SmoInput
            {
                Lambda = lambda,
                LambdaUnit = lambdaUnit,
                Mu = useMu ? serviceValue : null,
                MuUnit = serviceUnit,
                TObs = useMu ? null : serviceValue,
                TObsUnit = serviceUnit,
                Channels = channels,
                QueueLength = queueLength
            };

            SmoResult result = SmoCalculator.Calculate(_type, input);
            if (!result.Success)
            {
                ErrorText.Text = "Ошибка: " + result.Error;
                return;
            }

            _lastInput = input;
            _lastResult = result;

            var steps = new StringBuilder();
            foreach (CalculationStep step in result.Steps)
                steps.AppendLine(step.ToString());
            StepsText.Text = steps.ToString();

            string unit = lambdaUnit.Ru();
            var sb = new StringBuilder();
            sb.AppendLine($"ρ = {ObshieVichislia.Format(result.Rho)}");
            sb.AppendLine($"P0 = {ObshieVichislia.Format(result.P0)}");
            sb.AppendLine($"P_отк = {ObshieVichislia.Format(result.P_otk)}");
            sb.AppendLine($"Q = {ObshieVichislia.Format(result.Q)}");
            sb.AppendLine($"A = {ObshieVichislia.Format(result.A)} заявок/{unit}");
            if (result.P_wait.HasValue)
                sb.AppendLine($"P_ожид = {ObshieVichislia.Format(result.P_wait.Value)}");
            sb.AppendLine($"L_оч = {ObshieVichislia.Format(result.L_och)}");
            sb.AppendLine($"T_оч = {ObshieVichislia.Format(result.T_och)} {unit}");
            sb.AppendLine($"L_сист = {ObshieVichislia.Format(result.L_sist)}");
            sb.AppendLine($"T_сист = {ObshieVichislia.Format(result.T_sist)} {unit}");
            if (result.K_zan.HasValue)
                sb.AppendLine($"K_зан = {ObshieVichislia.Format(result.K_zan.Value)}");
            ResultsText.Text = sb.ToString();
        }

        private void Export_Click(object sender, RoutedEventArgs e)
        {
            ExcelPackage.License.SetNonCommercialPersonal("MatMe");

            if (_lastResult is null || _lastInput is null)
            {
                ErrorText.Text = "Сначала выполните расчёт.";
                return;
            }

            var dialog = new SaveFileDialog
            {
                Filter = "Excel Workbook (*.xlsx)|*.xlsx",
                FileName = "SMO_Result.xlsx"
            };

            if (dialog.ShowDialog() != true)
                return;

            try
            {
                using var package = new ExcelPackage();
                ExcelWorksheet ws = package.Workbook.Worksheets.Add("Результаты");

                int row = 1;
                ws.Cells[row, 1].Value = TitleText.Text;
                ws.Cells[row, 1].Style.Font.Bold = true;
                ws.Cells[row, 1].Style.Font.Size = 14;
                row += 2;

                ws.Cells[row, 1].Value = "Исходные данные";
                ws.Cells[row, 1].Style.Font.Bold = true;
                row++;

                ws.Cells[row, 1].Value = "Интенсивность потока заявок λ";
                ws.Cells[row, 2].Value = _lastInput.Lambda;
                ws.Cells[row, 3].Value = "заявок/" + _lastInput.LambdaUnit.Ru();
                row++;

                if (_lastInput.Mu.HasValue)
                {
                    ws.Cells[row, 1].Value = "Интенсивность обслуживания μ";
                    ws.Cells[row, 2].Value = _lastInput.Mu.Value;
                    ws.Cells[row, 3].Value = "1/" + _lastInput.MuUnit.Ru();
                }
                else if (_lastInput.TObs.HasValue)
                {
                    ws.Cells[row, 1].Value = "Время обслуживания t_обс";
                    ws.Cells[row, 2].Value = _lastInput.TObs.Value;
                    ws.Cells[row, 3].Value = _lastInput.TObsUnit.Ru();
                }
                row++;

                bool isMulti = _type is SmoType.MultiRefusals
                    or SmoType.MultiLimitedQueue
                    or SmoType.MultiUnlimitedQueue;
                bool hasQueue = _type is SmoType.SingleLimitedQueue
                    or SmoType.MultiLimitedQueue;

                if (isMulti)
                {
                    ws.Cells[row, 1].Value = "Число каналов n";
                    ws.Cells[row, 2].Value = _lastInput.Channels;
                    row++;
                }

                if (hasQueue)
                {
                    ws.Cells[row, 1].Value = "Длина очереди m";
                    ws.Cells[row, 2].Value = _lastInput.QueueLength;
                    row++;
                }

                row++;

                ws.Cells[row, 1].Value = "Шаги расчёта";
                ws.Cells[row, 1].Style.Font.Bold = true;
                row++;

                ws.Cells[row, 1].Value = "Показатель";
                ws.Cells[row, 2].Value = "Формула";
                ws.Cells[row, 3].Value = "Значение";
                ws.Cells[row, 1, row, 3].Style.Font.Bold = true;
                row++;

                foreach (CalculationStep step in _lastResult.Steps)
                {
                    ws.Cells[row, 1].Value = step.Name;
                    ws.Cells[row, 2].Value = step.Formula;
                    ws.Cells[row, 3].Value = step.Value;
                    row++;
                }

                row++;

                ws.Cells[row, 1].Value = "Характеристики";
                ws.Cells[row, 1].Style.Font.Bold = true;
                row++;

                AddCharacteristic(ws, ref row, "ρ", _lastResult.Rho);
                AddCharacteristic(ws, ref row, "P0", _lastResult.P0);
                AddCharacteristic(ws, ref row, "P_отк", _lastResult.P_otk);
                AddCharacteristic(ws, ref row, "Q", _lastResult.Q);
                AddCharacteristic(ws, ref row, "A", _lastResult.A);
                if (_lastResult.P_wait.HasValue)
                    AddCharacteristic(ws, ref row, "P_ожид", _lastResult.P_wait.Value);
                AddCharacteristic(ws, ref row, "L_оч", _lastResult.L_och);
                AddCharacteristic(ws, ref row, "T_оч", _lastResult.T_och);
                AddCharacteristic(ws, ref row, "L_сист", _lastResult.L_sist);
                AddCharacteristic(ws, ref row, "T_сист", _lastResult.T_sist);
                if (_lastResult.K_zan.HasValue)
                    AddCharacteristic(ws, ref row, "K_зан", _lastResult.K_zan.Value);

                ws.Cells[ws.Dimension.Address].AutoFitColumns();
                ws.Column(2).Width = Math.Min(ws.Column(2).Width, 80);
                ws.Column(3).Width = Math.Min(ws.Column(3).Width, 20);

                package.SaveAs(new FileInfo(dialog.FileName));
                ErrorText.Text = "Файл сохранён: " + dialog.FileName;
            }
            catch (Exception ex)
            {
                ErrorText.Text = "Ошибка экспорта: " + ex.Message;
            }
        }

        private static void AddCharacteristic(ExcelWorksheet ws, ref int row, string name, double value)
        {
            ws.Cells[row, 1].Value = name;
            ws.Cells[row, 2].Value = value;
            row++;
        }

        private static TimeUnit SelectedUnit(ComboBox box) => box.SelectedIndex switch
        {
            0 => TimeUnit.Minute,
            1 => TimeUnit.Hour,
            2 => TimeUnit.Day,
            _ => TimeUnit.Hour
        };

        private static bool TryParseDouble(string? text, out double value)
        {
            value = 0;
            if (string.IsNullOrWhiteSpace(text))
                return false;

            string trimmed = text.Trim();
            return double.TryParse(trimmed, NumberStyles.Float, CultureInfo.CurrentCulture, out value)
                || double.TryParse(trimmed, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
        }

        private static string TypeName(SmoType type) => type switch
        {
            SmoType.SingleRefusals => "Одноканальная СМО с отказами в обслуживании",
            SmoType.SingleLimitedQueue => "Одноканальная СМО с ограниченной длиной очереди",
            SmoType.SingleUnlimitedQueue => "Одноканальная СМО с неограниченной очередью",
            SmoType.MultiRefusals => "Многоканальная СМО с отказами в обслуживании",
            SmoType.MultiLimitedQueue => "Многоканальная СМО с ограниченной длиной очереди",
            SmoType.MultiUnlimitedQueue => "Многоканальная СМО с неограниченной очередью",
            _ => type.ToString()
        };
    }
}
