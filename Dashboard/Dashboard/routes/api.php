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
Route::post('feedback', 'Api\LabyrintApiController@returnFeedback');
Route::get('statistics', 'Api\LabyrintApiController@getStatistics');


Route::get('overview', 'Api\StoryApiController@getOverview');
Route::get('stories', 'Api\StoryApiController@getStories');

Route::get('story/{storyId}', 'Api\StoryApiController@getStory');
Route::post('story', 'Api\StoryApiController@newStory');
Route::post('story/{storyId}/change', 'Api\StoryApiController@changeStory');
Route::get('testStory/new', function(){
    return view('test.story_insert');
});
Route::get('testStory', function(){
    $stories = \App\Story::get();
    return view('test.story_list', compact('stories'));
});
Route::get('testStory/change/{id}', function($id){
    $story = \App\Story::find($id);
    return view('test.story_change', compact('story'));
});
Route::delete('story/{storyId}', 'Api\StoryApiController@deleteStory');
Route::delete('storyItem/{storyItemId}', 'Api\StoryApiController@deleteStoryItem');
Route::get('story/{storyId}/{active}', 'Api\StoryApiController@storyChangeActive')->where([
    'active' => '(deactivate|activate)'
]);

Route::get('file/{fileId}', 'Api\LabyrintApiController@downloadFile');


