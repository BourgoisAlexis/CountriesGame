using System.Collections.Generic;

public enum PropertyType {
    MostRecent,
    NestedProp,
    Prop
}

public class PropertyQuery {
    public string id;
    public PropertyType type;
    public string idForQuery;
    public string unit;

    public PropertyQuery(string id, PropertyType type, string unit, string idForQuery = "") {
        this.id = id;
        this.type = type;
        this.unit = unit;
        this.idForQuery = idForQuery;
    }
}

public class AppConst {
    public string gameVersionKey = "gameVersion";
    public string gameVersion = "1.0";

    public string serverMessageError = "servermessage_error";
    public string serverMessageUserJoin = "servermessage_userjoin";
    public string serverMessageJoinRoom = "servermessage_joinroom";
    public string serverMessageLeaveRoom = "servermessage_leaveroom";
    public string serverMessageStartGame = "servermessage_startgame";
    public string serverMessageSelectTheme = "servermessage_selecttheme";
    public string serverMessageCurrentPlayer = "servermessage_currentplayer";
    public string serverMessageDrawCard = "servermessage_drawcard";
    public string serverMessagePlayCard = "servermessage_playcard";
    public string serverMessageProcessQuery = "servermessage_processquery";
    public string serverMessageContestResult = "servermessage_contestresult";
    public string serverMessageNextRound = "servermessage_nextround";
    public string serverMessageGameEnded= "servermessage_gameended";
    public string serverMessageReturnToLobby = "servermessage_returntolobby";

    //Themes
    public List<PropertyQuery> themes = new List<PropertyQuery>() {
        new PropertyQuery("P2250", PropertyType.MostRecent, "years"),//life_expectancy
        //new PropertyQuery("P571", PropertyType.Prop, ""),//"inception",
        //"number_of_official_languages",
        //"latitude",//(north / south)
        //"longitude",//(east / west)
        new PropertyQuery("P610", PropertyType.NestedProp, "m", "P610 [wdt:P2044 ?value]."),//highest_point
        //new PropertyQuery("P1589", PropertyType.NestedProp, "m", "P1589 [wdt:P2044 ?value].")//lowest_point
        //"age_of_candidacy",
        //"age_of_consent",
        //"age_of_majority",
        //"retirement_age",
        new PropertyQuery("P1082", PropertyType.MostRecent, "ppl"),//population
        //"parity_percentage",//(female_population / population * 100)
        //"nominal_GDP",//(PIB)
        //"P1081",//human_development_index
        //"suicide_rate",
        //"unemployment_rate",
        //"median_income",
        new PropertyQuery("P2046", PropertyType.Prop, "km²"),//"area",
        //"perimeter",
        //"number_of_household",
        //"number_of_outofschool_children",
        //"P4442"//"mean_age",
        new PropertyQuery("P4841", PropertyType.MostRecent, ""),//"fertility_rate",
        //"P6591"//"maximum_temperature_record",
        //"minimum_wage",
        //"literacy_rate",
        //"minimum_temperature_record",
        //"democracy_index",
        //"happy_planet_index_score",
        //"country_calling_code",
    };
}

