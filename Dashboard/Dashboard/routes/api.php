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
Route::get('statistics', 'Api\LabyrintApiController@getStatistics');


Route::get('overview', 'Api\StoryController@getOverview');
Route::get('stories', 'Api\StoryController@getStories');
Route::get('test_story', 'Api\StoryController@testStory');
Route::get('test', function(){
    return view('test_story_view');
});
Route::put('story', 'Api\StoryController@newStory');
Route::patch('story/{storyId}', 'Api\StoryController@changeStory');
Route::delete('story/{storyId}', 'Api\StoryController@deleteStory');


