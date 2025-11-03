using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;

public class VSNotifyContext
{
    private readonly object _target;
    private readonly Lazy<Action<string>?> _lazyRaise;

    public VSNotifyContext(object target)
    {
        var all = target.GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic);
        var list = all.Where(m => m.Name == "OnPropertyChanged").ToList();
        Debug.WriteLine($"찾은 OnPropertyChanged: {list.Count}개");
        foreach (var method in list)
        {
            Debug.WriteLine($"🧭 시그니처: {method}");
        }
        _target = target;

        _lazyRaise = new Lazy<Action<string>?>(() =>
        {
            var method = _target.GetType()
                .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
                .FirstOrDefault(m =>
                    m.Name == "OnPropertyChanged" &&
                    m.GetParameters().Length == 1 &&
                    m.GetParameters()[0].ParameterType == typeof(string));

            if (method != null)
                return prop => method.Invoke(_target, new object[] { prop });

            Debug.WriteLine($"⚠️ 정확한 OnPropertyChanged(string) 메서드를 찾지 못했습니다.");
            return null;
        });
    }

    public void Raise(string propertyName)
    {
        try
        {
            var raise = _lazyRaise.Value;
            Debug.WriteLine($"👉 Raise delegate is {(raise != null ? "READY" : "NULL")}");
            _lazyRaise.Value?.Invoke(propertyName);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"❌ Notification failed for {propertyName}: {ex.Message}");
        }
    }
}