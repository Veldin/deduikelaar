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


use Illuminate\Support\Facades\Artisan;
use Illuminate\Support\Facades\DB;
use Illuminate\Support\Facades\Route;
use Illuminate\Support\Facades\Schema;

Route::get('/', 'Api\LabyrintApiController@documentation');
Route::get('order', 'Api\LabyrintApiController@getOrder');
Route::get('feedback', 'Api\LabyrintApiController@getFeedback');
Route::post('feedback', 'Api\LabyrintApiController@returnFeedback');
Route::get('statistics', 'Api\LabyrintApiController@getStatistics');


Route::get('overview', 'Api\StoryApiController@getOverview');
Route::get('stories', 'Api\StoryApiController@getStories');

Route::prefix('story')->group(function() {
    Route::get('{storyId}/preview', 'Api\StoryApiController@previewStory');
    Route::get('{storyId}', 'Api\StoryApiController@getStory');
    Route::post('{storyId}/change', 'Api\StoryApiController@changeStory');
    Route::post('/', 'Api\StoryApiController@newStory');
});
Route::prefix('testStory')->group(function(){

    Route::get('/', function(){
        $stories = \App\Story::get();
        return view('test.story_list', compact('stories'));
    });

    Route::get('new', function(){
        return view('test.story_insert');
    });
    Route::get('change/{id}', function($id){
        $story = \App\Story::find($id);
        return view('test.story_change', compact('story'));
    });
    Route::get('example/{id}', function($id){
//        $story = \App\Story::find($id);
        return view('test.story_view', compact('id'));
    });
    Route::get('view/{id}', function($id){
        $story = \App\Story::with('storyItems')->find($id);
        return view('information-piece', compact('story'));
    });
});
Route::delete('story/{storyId}', 'Api\StoryApiController@deleteStory');
Route::delete('storyItem/{storyItemId}', 'Api\StoryApiController@deleteStoryItem');
Route::get('story/{storyId}/{active}', 'Api\StoryApiController@storyChangeActive')->where([
    'active' => '(deactivate|activate)'
]);

Route::post('file/convert', 'Api\LabyrintApiController@convertFile');
Route::get('file/{fileId}', 'Api\LabyrintApiController@downloadFile');

Route::get('database/refresh', function(){

    if(Schema::hasTable('migrations')){
        $migration = DB::table('migrations')->orderBy('batch', 'DESC')->first();

        for($i=0;$i<$migration->batch;$i++){
            try{
                Artisan::call('migrate:rollback');
            }catch (Exception $e){

            }
        }
    }


    Artisan::call('migrate');
    Artisan::call('db:seed');
});

Route::get('database/fill', function(){
    Artisan::call('db:seed');
});
