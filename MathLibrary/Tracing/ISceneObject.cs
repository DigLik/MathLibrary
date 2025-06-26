using MathLibrary.Geometry;

namespace MathLibrary.Tracing
{
    /// <summary>
    /// Интерфейс для любого объекта, который может быть помещен в сцену и пересечен лучом.
    /// </summary>
    public interface ISceneObject
    {
        /// <summary>
        /// Проверяет, пересекает ли луч данный объект.
        /// </summary>
        /// <param name="ray">Луч для проверки.</param>
        /// <param name="tMin">Минимальное расстояние для валидного пересечения.</param>
        /// <param name="tMax">Максимальное расстояние для валидного пересечения.</param>
        /// <param name="hitInfo">Информация о попадании.</param>
        /// <returns>True, если пересечение найдено в заданном диапазоне.</returns>
        bool Intersect(Ray ray, float tMin, float tMax, out HitInfo hitInfo);
    }
}
