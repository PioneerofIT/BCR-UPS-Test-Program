using System.Collections;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using VSP.GUI.BASE_CONTROL;
namespace VSP.COMMON.Base_Component
{
    public class VSBindProperty
    {
        private readonly Dictionary<string, FrameworkElement> _elements = new();
        private readonly Dictionary<FrameworkElement, List<Action<object?>>> _bindings = new();
        private readonly Dictionary<string, object?> _values = new();

        public VSBindProperty(object codeBehind, bool autoBindDefaults = true)
        {
            RegisterAll(codeBehind);
            if (autoBindDefaults)
                BindDefaults();
        }

        private void RegisterAll(object context)
        {
            var fields = context.GetType()
                .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (var field in fields)
            {
                if (field.GetValue(context) is FrameworkElement element &&
                    !string.IsNullOrWhiteSpace(element.Name))
                {
                    _elements[element.Name] = element;
                }
            }
        }

        public void BindDefaults()
        {
            foreach (var (_, element) in _elements)
            {
                switch (element)
                {
                    case TextBlock tb:
                        Bind(tb, v =>
                        {
                            Debug.WriteLine($"[Bind] TextBlock value type: {v?.GetType().FullName ?? "null"}, value: {v ?? "null"}");
                            tb.Text = v?.ToString();
                        });
                        break;

                    case TextBox txt:
                        Bind(txt, v =>
                        {
                            Debug.WriteLine($"[Bind] TextBox value type: {v?.GetType().FullName ?? "null"}, value: {v ?? "null"}");
                            txt.Text = v?.ToString();
                        });
                        break;

                    case CheckBox cb:
                        Bind(cb, v =>
                        {
                            Debug.WriteLine($"[Bind] CheckBox value type: {v?.GetType().FullName ?? "null"}, value: {v ?? "null"}");
                            cb.IsChecked = v as bool?;
                        });
                        break;

                    case VSAdvEdit EditTxt:
                        Bind(EditTxt, v =>
                        {
                            Debug.WriteLine($"[Bind] VSAdvEdit value type: {v?.GetType().FullName ?? "null"}, value: {v ?? "null"}");
                            EditTxt.Text = v?.ToString();
                        });
                        break;

                    case VSAdvStringGrid grid:
                        Bind(grid, v =>
                        {
                            try
                            {
                                Debug.WriteLine($"[Bind] VSAdvStringGrid value type: {v?.GetType().FullName ?? "null"}, value: {v ?? "null"}");
                                grid.DataSource = v as IEnumerable;
                                grid.UpdateLayout();
                                //Debug.WriteLine($"[VSBind] ✅ VSAdvStringGrid 핸들러 등록됨! 해시: {grid.GetHashCode()}");

                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine($"[Bind] VSAdvStringGrid 예외: {ex.Message}");
                            }
                        });                     
                        break;
                    case DataGrid dg:
                        Bind(dg, v => dg.ItemsSource = v as IEnumerable);
                        break;
                    case ItemsControl ic:
                        Bind(ic, v =>
                        {
                            Debug.WriteLine($"[Bind] ItemsControl value type: {v?.GetType().FullName ?? "null"}, value: {v ?? "null"}");
                            ic.ItemsSource = v as IEnumerable;
                        });
                        break;
                }
            }
        }

    public void Bind(FrameworkElement element, Action<object?> handler)
    {
    if (!_bindings.TryGetValue(element, out var handlerList))
    {
        handlerList = new List<Action<object?>>();
        _bindings[element] = handlerList;
    }

    // 동일한 핸들러 중복 등록 방지
    if (!handlerList.Contains(handler))
        handlerList.Add(handler);

    // 해당 element가 이름으로 등록되어 있을 경우, 값이 있다면 즉시 반영
    var key = _elements.FirstOrDefault(kv => kv.Value == element).Key;

    if (!string.IsNullOrWhiteSpace(key) && _values.TryGetValue(key, out var storedValue))
    {
        try
        {
            handler(storedValue);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[VSBindProperty] 초기 값 핸들러 실행 예외 ({key}): {ex.Message}");
        }
    }
}

        public void Set(string key, object? value)
        {
            if (_elements.TryGetValue(key, out var element))
                SetInternal(key, element, value);
        }

        public void SetByObject(FrameworkElement element, object? value)
        {
            var key = _elements.FirstOrDefault(kv => kv.Value == element).Key;

            if (!string.IsNullOrWhiteSpace(key))
            {
                SetInternal(key, element, value);
            }
            else if (_bindings.TryGetValue(element, out var fallbackList))
            {
                foreach (var handler in fallbackList)
                {
                    try { handler(value); }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"[VSBindProperty] 수동 바인딩 예외: {ex.Message}");
                    }
                }
            }
        }

        private void SetInternal(string key, FrameworkElement element, object? value)
        {

            if (!_bindings.TryGetValue(element, out var handlers))
                return;

                if (element is DataGrid or VSAdvStringGrid)
                {
                    if (value is IList rawList && value?.GetType().IsGenericType == true)
                    {
                        var itemType = value.GetType().GetGenericArguments()[0];
                        var ocType = typeof(ObservableCollection<>).MakeGenericType(itemType);
                        var collection = (IList?)Activator.CreateInstance(ocType);

                        if (collection != null)
                        {
                            foreach (var item in rawList)
                            {
                                collection.Add(item); // 🧩 IList 인터페이스에 Add 있음
                            }

                            value = collection;
                        }
                        else
                        {
                            Debug.WriteLine($"[VSBind] ❗ ObservableCollection<{itemType.Name}> 생성 실패");
                        }
                    }

                    if (_values.TryGetValue(key, out var prev) &&
                        AreCollectionsEqual(prev as IEnumerable, value as IEnumerable))
                    {

                        //Debug.WriteLine($"[Debug] SetInternal key: {key}, prevVal: {prev?.ToString() ?? "null"}");
                        //Debug.WriteLine($"[Debug] _values.ContainsKey: {_values.ContainsKey(key)}");

                    return;
                    }
                }
                else
                {
                    if (_values.TryGetValue(key, out var prevVal) && Equals(prevVal, value))
                    {
                        //Debug.WriteLine($"[Debug] SetInternal key: {key}, prevVal: {prevVal?.ToString() ?? "null"}");
                        //Debug.WriteLine($"[Debug] _values.ContainsKey: {_values.ContainsKey(key)}");

                        return;
                    }

            }


                _values[key] = value;

            foreach (var handler in handlers)
            {
                try { handler(value); }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[VSBindProperty] 바인딩 예외 ({key}): {ex.Message}");
                }
            }
        }
        private bool AreCollectionsEqual(IEnumerable? a, IEnumerable? b)
        {
            if (ReferenceEquals(a, b)) return true;
            if (a == null || b == null) return false;

            var enumA = a.GetEnumerator();
            var enumB = b.GetEnumerator();

            try
            {
                while (true)
                {
                    bool hasA = enumA.MoveNext();
                    bool hasB = enumB.MoveNext();

                    if (hasA != hasB) return false;
                    if (!hasA) break;

                    var itemA = enumA.Current;
                    var itemB = enumB.Current;

                    if (!AreValuesEqual(itemA, itemB))
                        return false;
                }

                return true;
            }
            finally
            {
                (enumA as IDisposable)?.Dispose();
                (enumB as IDisposable)?.Dispose();
            }
        }

        private bool AreValuesEqual(object? a, object? b)
        {
            if (ReferenceEquals(a, b)) return true;
            if (a == null || b == null) return false;

            // 둘 다 IEnumerable일 경우 재귀 비교
            if (a is IEnumerable ea && b is IEnumerable eb)
                return AreCollectionsEqual(ea, eb);

            // 기본 Equals 비교 (참조형일 경우 override된 Equals도 자동 활용됨)
            return Equals(a, b);
        }

    }
}