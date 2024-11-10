namespace Ctrl;

public partial class StepperControl : ContentView
{
    #region 可绑定属性定义
    public static readonly BindableProperty MinimumProperty =
        BindableProperty.Create(nameof(Minimum), typeof(int), typeof(StepperControl), 0, BindingMode.OneTime);
    public int Minimum
    {
        get => (int)GetValue(MinimumProperty);
        set => SetValue(MinimumProperty, value);
    }

    public static readonly BindableProperty MaximumProperty =
        BindableProperty.Create(nameof(Maximum), typeof(int), typeof(StepperControl), 100, BindingMode.OneTime);
    public int Maximum
    {
        get => (int)GetValue(MaximumProperty);
        set => SetValue(MaximumProperty, value);
    }

    public static readonly BindableProperty IncrementProperty =
        BindableProperty.Create(nameof(Increment), typeof(int), typeof(StepperControl), 1, BindingMode.OneTime);
    public int Increment
    {
        get => (int)GetValue(IncrementProperty);
        set => SetValue(IncrementProperty, value);
    }

    public static readonly BindableProperty IsEditableProperty =
        BindableProperty.Create(nameof(IsEditable), typeof(bool), typeof(StepperControl), true, BindingMode.OneTime);
    public bool IsEditable
    {
        get => (bool)GetValue(IsEditableProperty);
        set => SetValue(IsEditableProperty, value);
    }

    public static readonly BindableProperty ValueProperty =
        BindableProperty.Create(nameof(Value), typeof(int), typeof(StepperControl), 0, BindingMode.TwoWay);
    public int Value
    {
        get => (int)GetValue(ValueProperty);
        set
        {
            SetValue(ValueProperty, value);
        }
    }

    public static readonly BindableProperty TextColorProperty =
        BindableProperty.Create(nameof(TextColor), typeof(Color), typeof(StepperControl), Colors.LightBlue, BindingMode.OneWay);
    public Color TextColor
    {
        get => (Color)GetValue(TextColorProperty);
        set => SetValue(TextColorProperty, value);
    }

    #endregion

    public StepperControl()
    {
        InitializeComponent();
    }

    private void OnMinusClicked(object sender, EventArgs e)
    {
        if (!IsEditable) return;
        if (Value - Increment >= Minimum) Value -= Increment;
    }
    private void OnPlusClicked(object sender, EventArgs e)
    {
        if (!IsEditable) return;
        if (Value + Increment <= Maximum) Value += Increment;
    }
}