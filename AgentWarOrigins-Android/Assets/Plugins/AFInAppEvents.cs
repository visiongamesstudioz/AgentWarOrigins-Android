using UnityEngine;
using System.Collections;

public class AFInAppEvents {
	/**
	 * Event Type
	 * */
	public const string LEVEL_ACHIEVED = "af_level_achieved";
	public const string ADD_PAYMENT_INFO = "af_add_payment_info";
	public const string ADD_TO_CART = "af_add_to_cart";
	public const string ADD_TO_WISH_LIST = "af_add_to_wishlist";
	public const string COMPLETE_REGISTRATION = "af_complete_registration";
	public const string TUTORIAL_COMPLETION = "af_tutorial_completion";
	public const string INITIATED_CHECKOUT = "af_initiated_checkout";
	public const string PURCHASE = "af_purchase";
	public const string RATE = "af_rate";
	public const string SEARCH = "af_search";
	public const string SPENT_CREDIT = "af_spent_credits";
	public const string ACHIEVEMENT_UNLOCKED = "af_achievement_unlocked";
	public const string CONTENT_VIEW = "af_content_view";
	public const string TRAVEL_BOOKING = "af_travel_booking";
	public const string SHARE = "af_share";
	public const string INVITE = "af_invite";
	public const string LOGIN = "af_login";
	public const string RE_ENGAGE = "af_re_engage";
	public const string UPDATE = "af_update";
	public const string OPENED_FROM_PUSH_NOTIFICATION = "af_opened_from_push_notification";
	public const string LOCATION_CHANGED = "af_location_changed";
	public const string LOCATION_COORDINATES = "af_location_coordinates";
	public const string ORDER_ID = "af_order_id";


    //custom events

    public const string VIEW_SETTINGS = "af_view_settings";
    public const string VIEW_STATS = "af_view_stats";
    public const string VIEW_MISSIONS = "af_view_mission";
    public const string VIEW_LEADERBOARD = "af_view_leaderboard";
    public const string CLICK_CRATES = "af_click_crates";
    public const string CLICK_INVENTORY = "af_click_inventory";
    public const string VIEW_STORE_PAGE = "af_view_store";
    public const string PURCHASE_COINS = "af_purchase_store_coins";
    public const string PURCHASE_DIAMONDS = "af_purchase_store_diamonds";
    public const string WATCH_VIDEO = "af_click_watch_video";
    public const string PLAY_AGAIN = "af_click_play_again";
    public const string RESUME_GAME_DIAMONDS = "af_click_resume_game_diamond";
    public const string INAPP_WATCH_VIDEO = "af_click_inapp_watch_video";
    public const string CLICKED_BUY_AGENT = "af_click_buy_agent";
    public const string BOUGHT_AGENT = "af_bought_agent";
    public const string CLICKED_BUY_WEAPON = "af_click_buy_weapon";
    public const string BOUGHT_WEAPON = "af_bought_weapon";
    public const string CLICKED_UPGRADE_WEAPON = "af_clicked_upgrade_weapon";
    public const string UPGRADED_WEAPON = "af_upgraded_weapon";
    public const string CLICKED_BUYBLUEPRINTCRATE = "af_click_buy_blueprint_crate";
    public const string BOUGHT_BLUEPRINTCRATE = "af_bought_blueprintcrate";
    public const string CLICK_DOUBLETOKENS = "af_click_double_tokens";
    public const string MISSION_COMPLETED = "af_mission_completed";

    public const string SKIP_TUTORIAL = "af_skip_tutorial";

    public const string CLICK_RATE_NOW = "af_click_rate_now";
    /**
	 * Event Parameter Name
	 * **/
    public const string LEVEL = "af_level";
	public const string SCORE = "af_score";
	public const string SUCCESS = "af_success";
	public const string PRICE = "af_price";
	public const string CONTENT_TYPE = "af_content_type";
	public const string CONTENT_ID = "af_content_id";
	public const string CONTENT_TITLE = "af_content_title";
    public const string NO_OF_UPGRADES = "af_no_of_upgrades";
	public const string CURRENCY = "af_currency";
	public const string QUANTITY = "af_quantity";
	public const string REGSITRATION_METHOD = "af_registration_method";
	public const string PAYMENT_INFO_AVAILIBLE = "af_payment_info_available";
	public const string MAX_RATING_VALUE = "af_max_rating_value";
	public const string RATING_VALUE = "af_rating_value";
	public const string SEARCH_STRING = "af_search_string";
	public const string DATE_A = "af_date_a";
	public const string DATE_B = "af_date_b";
	public const string DESTINATION_A = "af_destination_a";
	public const string DESTINATION_B = "af_destination_b";
	public const string DESCRIPTION = "af_description";
	public const string CLASS = "af_class";
	public const string EVENT_START = "af_event_start";
	public const string EVENT_END = "af_event_end";
	public const string LATITUDE = "af_lat";
	public const string LONGTITUDE = "af_long";
	public const string CUSTOMER_USER_ID = "af_customer_user_id";
	public const string VALIDATED = "af_validated";
	public const string REVENUE = "af_revenue";
	public const string RECEIPT_ID = "af_receipt_id";
    
	public const string PARAM_1 = "af_param_1";
	public const string PARAM_2 = "af_param_2";
	public const string PARAM_3 = "af_param_3";
	public const string PARAM_4 = "af_param_4";
	public const string PARAM_5 = "af_param_5";
	public const string PARAM_6 = "af_param_6";
	public const string PARAM_7 = "af_param_7";
	public const string PARAM_8 = "af_param_8";
	public const string PARAM_9 = "af_param_9";
	public const string PARAM_10 = "af_param_10";

    
}