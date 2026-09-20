using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.Splines;

public enum StageResult
{
    None,
    Win,
    Lose
}

public class Level_Main : MonoBehaviour
{
    [Header("Cart")]
    [SerializeField] private GameObject cart;
    [SerializeField] private SplineContainer rail;
    [SerializeField] private float railMoveSpeed = 5.0f;

    //[Header("State")]
    //[SerializeField] private bool isShelter

    void Start()
    {
        GameFlowAsync().Forget();
    }

    #region Game Flow

    async UniTask GameFlowAsync()
    {
        // 초기화
        GameManager.Instance.CurrentStageNumber = 0;
        GameManager.Instance.CurrentPlayerHealth = 3;

        while (true)
        {
            // 스테이지 시작
            StageResult result = await RunStageAsyc();

            // 스테이지 결과
            switch (result)
            {
                case StageResult.None:
                    break;
                case StageResult.Win:
                    // 쉼터 갔다가
                    await RunShelterAsync();
                    // 다음 스테이지 시작
                    break;
                case StageResult.Lose:
                    // 다시 시작?
                    break;
                default:
                    break;
            }
        }
    }

    async UniTask<StageResult> RunStageAsyc()
    {
        // 스테이지 초기화
        GameManager.Instance.CurrentStageNumber++;

        // 전투 시작
        StageResult result = await RunBattleAsync();

        // 전투 결과
        switch (result)
        {
            case StageResult.None:
                break;
            case StageResult.Win:
                break;
            case StageResult.Lose:
                break;
            default:
                break;
        }

        return result;
    }

    async UniTask<StageResult> RunBattleAsync()
    {
        var cts = new CancellationTokenSource();

        // 몬스터 스폰
        SpawnMonstersLoopAsync(cts.Token).Forget();

        // 광차 도착하거나 플레이어 죽을 때까지 대기
        var resultSource = new UniTaskCompletionSource<StageResult>();
        MoveCartToEndAsync(cts.Token, resultSource).Forget();
        PlayerDeadAsync(cts.Token, resultSource).Forget();

        StageResult result = await resultSource.Task;

        // 마무리
        cts.Cancel();

        ClearRemainingMonsters();

        return result;
    }

    async UniTask RunShelterAsync()
    {
        // 전리품 랜덤 생성

        // 유저가 준비가 다 될 때까지 대기

        await UniTask.CompletedTask;
    }

    #endregion

    async UniTask SpawnMonstersLoopAsync(CancellationToken token)
    {
        try
        {
            while (true)
            {
                // 몬스터 생성
                
                await UniTask.Delay(TimeSpan.FromSeconds(3), cancellationToken: token);
            }
        }
        catch
        {
        }
    }
    
    void ClearRemainingMonsters()
    {

    }

    async UniTask MoveCartToEndAsync(CancellationToken token, UniTaskCompletionSource<StageResult> source)
    {
        try
        {
            float totalLength = rail.CalculateLength();
            float currentDistance = 0f;

            while (currentDistance < totalLength)
            {
                currentDistance += railMoveSpeed * Time.deltaTime;

                // 어디까지 왔는지 비율
                float progress = Mathf.Clamp01(currentDistance / totalLength);

                // 비율에 따른 위치, 회전
                Vector3 position = rail.EvaluatePosition(progress);
                Vector3 tangent = rail.EvaluateTangent(progress);

                cart.transform.position = position;
                if (tangent != Vector3.zero)
                {
                    cart.transform.rotation = Quaternion.LookRotation(tangent);
                }

                await UniTask.Yield(PlayerLoopTiming.Update, token);
            }

            source.TrySetResult(StageResult.Win);
        }
        catch
        {
        }
    }

    async UniTask PlayerDeadAsync(CancellationToken token, UniTaskCompletionSource<StageResult> source)
    {
        try
        {
            // 플레이어 죽는 거 대기
            await UniTask.WaitUntil(() => (GameManager.Instance.CurrentPlayerHealth <= 0));
            
            source.TrySetResult(StageResult.Lose);
        }
        catch
        {
        }
    }
}
