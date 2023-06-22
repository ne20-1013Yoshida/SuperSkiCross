public interface IRankingDecider
{
    float ReturnWaypointIndex(); //次のwaypointのindexを返す
    float MeasureWaypointDistance(); //次のwaypointまでの距離を返す
    void DecideRanking(int ranking); //順位を与える
}
