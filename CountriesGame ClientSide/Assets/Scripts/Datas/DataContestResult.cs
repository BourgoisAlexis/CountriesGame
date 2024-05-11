public class DataContestResult {
    public bool result { get; private set; }
    public string value { get; private set; }

    public DataContestResult(bool result, string value) {
        this.result = result;
        this.value = value;
    }
}