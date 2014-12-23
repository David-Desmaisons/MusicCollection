using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicCollectionWPF.ViewModel.Interface
{
    /// <summary>
    /// Compute distance beetween an object and an reference object
    /// </summary>
    public interface IDistanceEvaluator<T>
    {
        /// <summary>
        /// Should be called when distance evaluator is caching data from centered object
        /// and centered object changed
        /// </summary>   
        void UpdateCacheData();

        /// <summary>
        /// Compute distance beetween an object and the entity repesented by the interface
        /// </summary>   
        /// <returns> 
        /// the corresponding distance 
        /// </returns>
        int EvaluateDistance(T iElement);

        /// <summary>
        /// Get the Object from which the distance is computed
        /// </summary>   
        /// <returns> 
        /// Object from which the distance is computed
        /// </returns>
        T Reference { get; }

    }
}
