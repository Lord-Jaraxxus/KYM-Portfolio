using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace KYM
{
    [System.Serializable]
    public class EffectData 
    {
        public string key;
        public float lifeTime = 5f;
        public GameObject prefab;
    }

    public class EffectManager : MonoBehaviour
    {
        public static EffectManager Instance;

        [SerializeField] private List<EffectData> effectList = new List<EffectData>();
        private Dictionary<string, ObjectPool<GameObject>> effectPools = new();

        private void Awake() => Instance = this;
        private void OnDestroy() => Instance = null;

        void Start()
        {
            // 이펙트 풀 초기화
            foreach (var effectData in effectList)
            {
                var pool = new ObjectPool<GameObject>(
                    createFunc: () => Instantiate(effectData.prefab),   // 새 오브젝트가 필요할때 호출됨
                    actionOnGet: obj => obj.SetActive(true),            // 오브젝트를 풀에서 꺼낼 때 호출됨
                    actionOnRelease: obj => obj.SetActive(false),       // 오브젝트를 다시 넣을 때 호출됨
                    actionOnDestroy: obj => Destroy(obj),               // 오브젝트가 파괴될 때 호출됨
                    defaultCapacity: 10,                                // 초기 풀 크기 설정
                    maxSize: 100                                        // 최대 풀 크기 설정
                );
                effectPools[effectData.key] = pool;                     // 해당 키로 풀을 딕셔너리에 저장
            }
        }

        public void SpawnEffect(string key, Vector3 position, Quaternion rotation)
        {
            if (!effectPools.TryGetValue(key, out var pool)) return; // 해당 키의 이펙트 풀을 찾아서 pool 변수에 할당, 찾지 못하면 종료

            var newEffect = pool.Get(); // 풀에서 이펙트 오브젝트를 가져옴
            newEffect.transform.SetPositionAndRotation(position, rotation); // 위치와 회전 설정

            var targeteffectData = effectList.Find(x => x.key.Equals(key)); // 이펙트 데이터 찾기
            StartCoroutine(DespawnEffect(pool, newEffect, targeteffectData.lifeTime)); // 지정된 시간 후에 이펙트를 풀로 반환
        }

        private IEnumerator DespawnEffect(ObjectPool<GameObject> pool, GameObject effect, float delay)
        {
            yield return new WaitForSeconds(delay); // 지정된 시간만큼 대기
            pool.Release(effect); // 풀에 이펙트를 반환
        }
    }
}
