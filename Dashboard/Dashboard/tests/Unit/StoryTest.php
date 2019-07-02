<?php

namespace Tests\Unit;

use App\File;
use App\Http\Controllers\Api\StoryApiController;
use App\Story;
use App\StoryItem;
use finfo;
use Illuminate\Http\UploadedFile;
use Illuminate\Support\Facades\Storage;
use Tests\TestCase;
use Illuminate\Foundation\Testing\WithFaker;
use Illuminate\Foundation\Testing\RefreshDatabase;

class StoryTest extends TestCase
{

    // Base story data
    private $storyData = [
        'title' => 'Story for testing',
        'description' => 'Story description',
        'texts' => [
            'Test text 1',
            'Test text 2'
        ]
    ];


    // Get all class methods of StoryApiController class
    public function testAllClasses(){
        var_dump(get_class_methods(StoryApiController::class));

    // Methods to test
    // "getOverview"
    // "getStory"
    // "getStories"
    // "deleteStory"
    // "deleteStoryItem"
    }

    /**
     * Test deleting a story
     */
    public function testDeleteStory(){

        // Create a story
        $story = Story::create([
            'title' => 'name 1',
            'icon' => 'name 2'
        ]);
        $testStory = Story::find($story->id);
        if(!$testStory){
            $this->assertTrue(false);
            return;
        }

        $this->json('DELETE', '/api/v1/story/'.$story->id);

        $testStory = Story::find($story->id);

        $this->assertNull($testStory);

    }


    /**
     * Test deleting a story item
     */
    public function testDeleteStoryItem(){

        // Create a story
        $story = Story::create([
            'title' => 'name 1',
            'icon' => 'name 2'
        ]);
        // Create a story item
        $storyItem = StoryItem::create([
            'storyId' => $story->id,
        ]);
        $testStoryItem = StoryItem::find($storyItem->id);
        if(!$testStoryItem){
            $this->assertTrue(false);
            return;
        }

        $this->json('DELETE', '/api/v1/storyItem/'.$storyItem->id);

        $testStoryItem = StoryItem::find($storyItem->id);

        $this->assertNull($testStoryItem);

        $story->delete();

    }


    /**
     * Test changing activating or deactivating a story
     */
    public function testStoryChangeActive(){

        // Test failed when story not exists
        $response = $this->json('GET', '/api/v1/story/a/activate');
        $this->assertEquals('failed', $response->json()['response']);

        // Create a story
        $story = Story::create([
            'title' => 'name 1',
            'icon' => 'name 2',
        ]);

        // Set active and id
        $active = $story->active;
        $id = $story->id;


        // Check if active is changed
        $this->json('GET', '/api/v1/story/'.$story->id.'/'.($story->active ? 'deactivate' : 'activate'));
        $story = Story::find($id);
        $this->assertNotEquals($active, $story->active);

        // Check if active is changed back
        $this->json('GET', '/api/v1/story/'.$story->id.'/'.($story->active ? 'deactivate' : 'activate'));
        $story = Story::find($id);
        $this->assertEquals($active, $story->active);


        // Delete created story
        $story->delete();

    }


    /**
     * Test creating a story
     */
    public function testNewStory()
    {

        // Files, all images
        $files = [
//            \Illuminate\Support\Facades\File::get(storage_path('Test document.docx')),
            UploadedFile::fake()->image('test1.qwe', 1920, 1080),
            UploadedFile::fake()->image('test2.jpg', 1920, 1080),
            UploadedFile::fake()->image('test3.jpg', 1920, 1080),
            UploadedFile::fake()->image('test4.jpg', 1920, 1080),
            UploadedFile::fake()->image('test5.jpg', 1920, 1080),
            UploadedFile::fake()->image('test6.jpg', 1920, 1080),
        ];

        // Set base data
        $data = $this->storyData;

        // add files
        $data['files'] = $files;

        // Test a failed request
        $response = $this->json('POST', '/api/v1/story', $data);
        $this->assertEquals('failed', $response->json()['response']);

        // Add icon to the request data
        $data['icon'] = "candle";

        // Get and set file data for an docx file
        $filename = 'Test document.docx';
        $file_path = storage_path($filename);
        $finfo = new finfo(16);

        // Replace first file for an docx file
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

        // Get and set file data for an pdf file
        $filename = 'Test document.pdf';
        $file_path = storage_path($filename);
        $finfo = new finfo(16);

        // Replace second file for an pdf file
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


        // Get and set file data for an mp4 file
        $filename = 'Test video.mp4';
        $file_path = storage_path($filename);
        $finfo = new finfo(16);

        // Replace third file for an mp4 file
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


        // Get and set file data for an mp4 file
        $filename = 'Test video2.mp4';
        $file_path = storage_path($filename);
        $finfo = new finfo(16);

        // Replace fourth file for an mp4 file
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

        // Get and set file data for an mp3 file
        $filename = 'Test audio.mp3';
        $file_path = storage_path($filename);
        $finfo = new finfo(16);

        // Replace fifth file for an mp3 file
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

        // Do request
        $response = $this->json('POST', '/api/v1/story', $data);


        // Check response
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

    /**
     *  Test changing a story
     */
    public function testChangeStory(){


        // Check failed
        $response = $this->json('POST', '/api/v1/story/-102/change', []);
        $this->assertEquals('failed', $response->json()['response']);




        // Add fake image files
        $files = [
//            \Illuminate\Support\Facades\File::get(storage_path('Test document.docx')),
            UploadedFile::fake()->image('test1.jpg'),
            UploadedFile::fake()->image('test2.jpg'),
            UploadedFile::fake()->image('test3.jpg'),
        ];

        // Get and set file data for an docx file
        $filename = 'Test document.docx';
        $file_path = storage_path($filename);
        $finfo = new finfo(16);

        // Replace first file for an docx file
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

        // Set base data
        $data = $this->storyData;

        // Add files to data
        $data['files'] = $files;

        // Add icon to data
        $data['icon'] = "candle";

        // Do and check request (create new story)
        $response = $this->json('POST', '/api/v1/story', $data);
        $this->assertEquals('success', $response->json()['response']);

        // Change story
        if(isset($response->json()['storyId'])){

            // Set id of the story
            $id = $response->json()['storyId'];

            // Set new data
            $data = [
                'title' => 'Changed title'
            ];

            // Do change request
            $response = $this->json('POST', '/api/v1/story/'.$id.'/change', $data);

            // Should be successful
            $this->assertEquals('success', $response->json()['response']);


            // Delete story
            $this->json('DELETE', '/api/v1/story/'.$id);
        }
    }

    /**
     * Test convert an pdf document
     */
    public function testConvertFile(){

        // Set file name
        $filename = 'Test document.pdf';

        // Get file path
        $file_path = storage_path($filename);

        // Get file info
        $finfo = new finfo(16);

        // Fake file as uploaded file
        $file = new \Illuminate\Http\UploadedFile(
            $file_path,
            $filename,
            $finfo->file($file_path),
            filesize($file_path),
            0,
            false
        );

        // Do request
        $response = $this->json('POST', '/api/v1/file/convert', ['file' => $file]);

        // Check response
        $this->assertEquals('success', $response->json()['response']);
    }
}

