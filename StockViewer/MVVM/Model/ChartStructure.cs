
namespace StockViewer.MVVM.Model;
public class ChartStructure<T> : ObservableObject
{
    protected enum Option { Left, Right }
    protected ObservableCollection<T> _Middle = new();
    protected Stack<T> _Left = new();
    protected Stack<T> _Right = new();
    protected QuantityHash _QuantityHash = new();
    protected Func<T, (double, double , DateTime)> _Func;

    public ObservableCollection<T> Middle { get => _Middle;  set { _Middle = value; OnPropertyChanged(); }}
    public double Max => _QuantityHash.GetMax();
    public double Min => _QuantityHash.GetMin();
    public bool AllShow => !_Left.Any() && !_Right.Any();



    public ChartStructure(Func<T, (double, double, DateTime)> func) => _Func = func;



    public void Clear() 
    {
        _Middle = new();
        _Right = new();
        _Left = new();
        _QuantityHash = new();
    }
    public int Count() => _Middle.Count();
    public void Refresh() => Middle = _Middle;
    public void Push(T obj) => _Left.Push(obj);
    public T ElementAt(int index) => _Middle.ElementAt(index);
    public int GetMaxVolume(Func<T, int> func) => _Middle.Max(x => func.Invoke(x));
    public void Generate(int count)
    {
        while (count-- >= 0) 
        {
            if(!_Left.Any())
                return;

            T obj = _Left.Pop();
            AddMiddle(obj, Option.Left);
        }
    }
    public void ZoomOut(int count)
    {
        while (count-- >= 0)
        {
            if (_Left.Any())
            {
                T obj = _Left.Pop();
                AddMiddle(obj, Option.Left);
            }
            else if (_Right.Any())
            {
                T obj = _Right.Pop();
                AddMiddle(obj, Option.Right);
            }
            else
                return;
        }
    }
    public bool ZoomIn(int count)
    {
        if (!_Middle.Any() || _Middle.Count() < count + 1)
            return false;
        while (count-- >= 0)
        {
            T obj = ReduceMiddle(Option.Left);
            _Left.Push(obj);
        }
        return true;
    }
    public void PanLeft(int count)
    {
        while (count-- >= 0 && _Left.Any())
        {
            T left = _Left.Pop();
            AddMiddle(left, Option.Left);

            T right = ReduceMiddle(Option.Right);
            _Right.Push(right);
        }
    }
    public void PanRight(int count)
    {
        while (count-- >= 0 && _Right.Any())
        {
            T right = _Right.Pop();
            AddMiddle(right, Option.Right);

            T left = ReduceMiddle(Option.Left);
            _Left.Push(left);
        }
    }



    protected void AddMiddle(T obj, Option option) 
    {
        (double max, double min, DateTime dateTime) = _Func.Invoke(obj);

        _QuantityHash.AddMax(max, dateTime);
        _QuantityHash.AddMin(min, dateTime);

        if (option == Option.Right)
            _Middle.Insert(0, obj);
        else
            _Middle.Add(obj);

        Refresh();
    }
    protected T ReduceMiddle(Option option)
    {
        int index = option == Option.Right ? 0 : _Middle.Count() - 1;

        T obj = option == Option.Right ? _Middle.First() : _Middle.Last();
        (double max, double min, DateTime dateTime) = _Func.Invoke(obj);
        _QuantityHash.RemoveMax(max, dateTime);
        _QuantityHash.RemoveMin(min, dateTime);

        if (option == Option.Right)
            _Middle.RemoveAt(0);
        else
            _Middle.RemoveAt(_Middle.Count - 1);

        Refresh();
        return obj;
    }
}
