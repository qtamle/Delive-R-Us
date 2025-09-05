namespace MHUtility
{
    using UnityEngine;
    
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        #region Setters/Private Variables

        [SerializeField] private bool _presistThroughScenes = false;
        public static T Instance { get; private set; }

        #endregion

        #region Unity Methods
        
        protected virtual void Awake()
        {
            if (Instance == null)
            {
                Instance = this as T;

                if (_presistThroughScenes)
                    DontDestroyOnLoad(gameObject);
            }
            else if (Instance != this)
            {
                Destroy(this.gameObject);
                return;
            }
        }

        #endregion
    }
}