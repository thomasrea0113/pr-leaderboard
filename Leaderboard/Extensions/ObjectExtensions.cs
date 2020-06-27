using System.Linq;

namespace Leaderboard.Extensions
{
    public static class ObjectExtensions
    {
        /// <summary>
        /// Maps the properties of the current object to a new instance of type <see cref="{T}" />
        /// </summary>
        /// <param name="obj"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T ToObject<T>(this object obj)
            where T : class, new()
        {
            var objProps = obj.GetType().GetProperties();
            var tProps = typeof(T).GetProperties();

            var likeProps = from op in objProps join tp in tProps on op.Name equals tp.Name select (op, tp);

            var newT = new T();
            foreach ((var op, var tp) in likeProps)
                tp.SetValue(newT, op.GetValue(obj));

            return newT;
        }
    }
}
