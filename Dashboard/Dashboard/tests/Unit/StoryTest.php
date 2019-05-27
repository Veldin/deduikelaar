<?php

namespace Tests\Unit;

use Illuminate\Http\UploadedFile;
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
            UploadedFile::fake()->image('test1.jpg'),
            UploadedFile::fake()->image('test2.docx'),
            UploadedFile::fake()->image('test3.jpg'),
        ];
        $data = $this->storyData;
        $data['files'] = $files;

        $response = $this->json('PUT', '/api/v1/story',$data);

        $this->assertEquals('failed',$response->json()['response']);

        $data['icon'] = "candle";
        $response = $this->json('PUT', '/api/v1/story',$data);

        if(isset($response->json()['response'])){
            $this->assertEquals('success',$response->json()['response']);
        }else{
            var_dump($response->json());
        }

        //        var_dump($response);
//        $this->assertTrue();
    }
}
