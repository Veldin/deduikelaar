<?php


/*
|--------------------------------------------------------------------------
| API Routes
|--------------------------------------------------------------------------
|
| Here is where you can register API routes for your application. These
| routes are loaded by the RouteServiceProvider within a group which
| is assigned the "api" middleware group. Enjoy building your API!
|
*/



Route::get('/', 'Api\LabyrintApiController@documentation');
Route::get('overview', 'Api\LabyrintApiController@getOverview');
Route::get('order', 'Api\LabyrintApiController@getOrder');
Route::get('stories', 'Api\LabyrintApiController@getStories');
Route::get('test_story', 'Api\LabyrintApiController@testStory');
Route::get('test', function(){
    return view('test_story_view');
});
Route::get('feedback', 'Api\LabyrintApiController@getFeedback');
Route::get('statistics', 'Api\LabyrintApiController@getStatistics');