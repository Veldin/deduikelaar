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


use Illuminate\Support\Facades\Route;

Route::get('/', 'Api\LabyrintApiController@documentation');
Route::get('order', 'Api\LabyrintApiController@getOrder');
Route::get('feedback', 'Api\LabyrintApiController@getFeedback');
Route::POST('feedback', 'Api\LabyrintApiController@returnFeedback');
Route::get('statistics', 'Api\LabyrintApiController@getStatistics');


Route::get('overview', 'Api\StoryApiController@getOverview');
Route::get('stories', 'Api\StoryApiController@getStories');
Route::get('test_story', 'Api\StoryApiController@testStory');
Route::get('test', function(){
    return view('test_story_view');
});
Route::POST('story', 'Api\StoryApiController@newStory');
Route::POST('story/{storyId}/change', 'Api\StoryApiController@changeStory');
Route::delete('story/{storyId}', 'Api\StoryApiController@deleteStory');


