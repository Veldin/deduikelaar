<?php

namespace Tests\Unit;

use App\File;
use App\Http\Controllers\Api\StoryController;
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

    /**
     * A basic unit test example.
     *
     * @return void
     */
    public function testNewStory()
    {

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

        $response = $this->json('PUT', '/api/v1/story', $data);

        $this->assertEquals('failed', $response->json()['response']);

        $data['icon'] = "candle";
        $response = $this->json('PUT', '/api/v1/story', $data);

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

        //        var_dump($response);
//        $this->assertTrue();
    }

//    public function testConvertWordFile(){
//
//        $file = \Illuminate\Support\Facades\File::get(storage_path('Test document.docx'));
//
//        $sc = new StoryController();
////        $data = $sc->convertWordFile($file);
//
////        assertEquals('','');
////        assertTrue(true);
//    }

}

