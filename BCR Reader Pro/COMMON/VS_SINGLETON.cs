using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//namespace VSP.COMMON
//{
//    public class Singleton<T> where T : class
//    {
//        private static readonly Lazy<T> instance = new(() =>
//            (T?)Activator.CreateInstance(typeof(T), true) ?? throw new InvalidOperationException("객체 생성 실패!")//파라미터 없는 생성자만 생성가능..
//        );

//        public static T Instance => instance.Value;
//    }
//}
namespace VSP.COMMON
{
    public class Singleton<T> where T : class
    {
        private static readonly T instance =
            (T?)Activator.CreateInstance(typeof(T), true)
            ?? throw new InvalidOperationException("객체 생성 실패!"); // 기본 생성자 필수

        public static T Instance => instance;
    }
}