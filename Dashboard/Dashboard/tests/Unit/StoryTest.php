<?php

namespace Tests\Unit;

use App\File;
use App\Http\Controllers\Api\StoryApiController;
use App\Story;
use finfo;
use Illuminate\Http\UploadedFile;
use Illuminate\Support\Facades\Storage;
use Tests\TestCase;
use Illuminate\Foundation\Testing\WithFaker;
use Illuminate\Foundation\Testing\RefreshDatabase;

class StoryTest extends TestCase
{

    private $storyData = [
        'title' => 'Story for testing',
        'description' => 'Story description',
        'texts' => [
            'Test text 1',
            'Test text 2'
        ]
    ];


    public function testAllClasses(){
        var_dump(get_class_methods(StoryApiController::class));

    // "getOverview"
    // "getStory"
    // "getStories"
    // "previewStory"
    // "deleteStory"
    // "deleteStoryItem"
    // "storyChangeActive"
    }


    public function testStoryChangeActive(){
//
//        Route::get('{storyId}/{active}', 'Api\StoryApiController@storyChangeActive')->where([
//            'active' => '(deactivate|activate)'

        $response = $this->json('GET', '/api/v1/story/a/activate');
//        var_dump($response->json());
//        var_dump($response->json()['response']);
        $this->assertEquals('failed', $response->json()['response']);

        $story = Story::first();
        $active = $story->active;
        $id = $story->id;


        $this->json('GET', '/api/v1/story/'.$story->id.'/'.($story->active ? 'deactivate' : 'activate'));
        $story = Story::find($id);
        // Check if active is changed
        $this->assertNotEquals($active, $story->active);

        // Check if active is changed back
        $this->json('GET', '/api/v1/story/'.$story->id.'/'.($story->active ? 'deactivate' : 'activate'));
        $story = Story::find($id);

        $this->assertEquals($active, $story->active);

    }


    /**
     * A basic unit test example.
     *
     * @return void
     */
    public function testNewStory()
    {

        $files = [
//            \Illuminate\Support\Facades\File::get(storage_path('Test document.docx')),
            UploadedFile::fake()->image('test1.qwe', 1920, 1080),
            UploadedFile::fake()->image('test2.jpg', 1920, 1080),
            UploadedFile::fake()->image('test3.jpg', 1920, 1080),
            UploadedFile::fake()->image('test4.jpg', 1920, 1080),
            UploadedFile::fake()->image('test5.jpg', 1920, 1080),
            UploadedFile::fake()->image('test6.jpg', 1920, 1080),
        ];

        $filename = 'Test document.docx';
        $file_path = storage_path($filename);
        $finfo = new finfo(16);

        $data = $this->storyData;
        $data['files'] = $files;

        $response = $this->json('POST', '/api/v1/story', $data);

        $this->assertEquals('failed', $response->json()['response']);


        // Replace first file docx
        if (Storage::disk('storage')->exists($filename)) {

            $data['files'][0] = new UploadedFile(
                $file_path,
                $filename,
                $finfo->file($file_path),
                filesize($file_path),
                0,
                false
            );
        }
        $filename = 'Test document.pdf';
        $file_path = storage_path($filename);
        $finfo = new finfo(16);
        // Replace second file pdf
        if (Storage::disk('storage')->exists($filename)) {

            $data['files'][1] = new UploadedFile(
                $file_path,
                $filename,
                $finfo->file($file_path),
                filesize($file_path),
                0,
                false
            );
        }
        $filename = 'Test video.mp4';
        $file_path = storage_path($filename);
        $finfo = new finfo(16);
        // Replace second file pdf
        if (Storage::disk('storage')->exists($filename)) {

            $data['files'][2] = new UploadedFile(
                $file_path,
                $filename,
                $finfo->file($file_path),
                filesize($file_path),
                0,
                false
            );
        }
        $filename = 'Test video2.mp4';
        $file_path = storage_path($filename);
        $finfo = new finfo(16);
        // Replace second file pdf
        if (Storage::disk('storage')->exists($filename)) {

            $data['files'][3] = new UploadedFile(
                $file_path,
                $filename,
                $finfo->file($file_path),
                filesize($file_path),
                0,
                false
            );
        }
        $filename = 'Test audio.mp3';
        $file_path = storage_path($filename);
        $finfo = new finfo(16);
        // Replace second file pdf
        if (Storage::disk('storage')->exists($filename)) {

            $data['files'][4] = new UploadedFile(
                $file_path,
                $filename,
                $finfo->file($file_path),
                filesize($file_path),
                0,
                false
            );
        }

        $data['icon'] = "candle";
        $response = $this->json('POST', '/api/v1/story', $data);

        if (isset($response->json()['response'])) {
            if($response->json()['response'] == "failed") var_dump($response->json()['errors']);
            $this->assertEquals('success', $response->json()['response']);

            if(isset($response->json()['storyId'])){
                // Delete story
                $this->json('DELETE', '/api/v1/story/'.$response->json()['storyId']);
            }
        } else {
            var_dump($response->json());
        }
    }

    public function testChangeStory(){


        // Check failed
        $response = $this->json('POST', '/api/v1/story/-102/change', []);

        $this->assertEquals('failed', $response->json()['response']);




        // Check success
        $files = [
//            \Illuminate\Support\Facades\File::get(storage_path('Test document.docx')),
            UploadedFile::fake()->image('test1.jpg'),
            UploadedFile::fake()->image('test2.jpg'),
            UploadedFile::fake()->image('test3.jpg'),
        ];

        $filename = 'Test document.docx';
        $file_path = storage_path($filename);
        $finfo = new finfo(16);

        if (Storage::disk('storage')->exists($filename)) {

            $files[0] = new UploadedFile(
                $file_path,
                $filename,
                $finfo->file($file_path),
                filesize($file_path),
                0,
                false
            );
        }

        $data = $this->storyData;
        $data['files'] = $files;
        $data['icon'] = "candle";
//        $data['files'] = $files;

        $response = $this->json('POST', '/api/v1/story', $data);
        $this->assertEquals('success', $response->json()['response']);

        if(isset($response->json()['storyId'])){
            $id = $response->json()['storyId'];
            $data = [
                'title' => 'Changed title'
            ];
            $response = $this->json('POST', '/api/v1/story/'.$id.'/change', $data);

            $this->assertEquals('success', $response->json()['response']);


            $this->json('DELETE', '/api/v1/story/'.$id);

        }
    }

}

