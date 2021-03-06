import React, {Component} from 'react';
import Popup from './popup/Popup';
import toastr from 'toastr';

import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faQuestionCircle } from '@fortawesome/free-solid-svg-icons';

import CKEditor from '@ckeditor/ckeditor5-react';
import ClassicEditor from '@ckeditor/ckeditor5-build-classic';

import CustomUploadAdapter from './plugins/CustomUploadAdapter.js'

class CreateItem extends Component {
  constructor() {
    super();
    this.state = {
      title: 'Vul hier de titel in',
      isChecked: true,
      editorContent: 'Vul hier de tekst in',
      imagesContent: '',
      files: null,
      removed: 0,
      showPopup1: false,
      showPopup2: false,
      showPopup3: false,
      showPopup4: false,
      text: [
        {title: 'Hier moet de titel ingevoerd worden, die duidelijk maakt waar dit specifieke item over gaat.'},
        {title: 'Hier kan gekozen worden voor een bestaand bestand toe te voegen of om zelf een stuk te schrijven.'},
        {title: 'Maak hier een eigen verhaal aan en voeg een afbeelding toe indien gewenst.'},
        {title: 'Kies hier het bestand dat u wilt toevoegen aan dit verhaal. Deze bestandstypen kunnen geüpload worden: docx, jpeg, jpg, png, gif, bmp, avi, mp4, mpeg, webm.'}
      ]      
    };

  }

  // Set custom upload adapter for CKeditor
  customAdapterPlugin( editor ) {
    editor.plugins.get( 'FileRepository' ).createUploadAdapter = ( loader ) => {
      return new CustomUploadAdapter( loader );
    };
  }

  changeState(event) {
    this.setState({
      title: event.target.value
    })
  }

  changeStateSwitch() {
    this.setState({
      isChecked: !this.state.isChecked
    });
  }

  // Sets the preview of the uploaded files
  setFile(e) {
    var t = this;
    this.setState({imagesContent: ""});
    var files = e.target.files; // Gets files that are selected
    // Loop through selected files to show them in preview
    for(var i=0;i<files.length;i++){
      var reader = new FileReader();
      reader.num = i;
      reader.readAsDataURL(files[i]);
      reader.onload = function (e) {
        var extension = files[this.num].name.split('.').pop().toLowerCase();
        // If pdf or docx, convert into usable format for preview
        // Else convert other files with base64
        if(['pdf','docx'].indexOf(extension) >= 0){
          var formData = new FormData();
          formData.append("file", files[this.num]);
          fetch('/api/v1/file/convert', {
            method: 'POST',
            body: formData
          }).then(response => response.json())
              .then(response => {
                if(response['response'] === "success"){
                  t.setState({ imagesContent: t.state.imagesContent + response['data'] });
                }else{
                  toastr.warning('Er kon geen voorbeeld van het bestand ' + files[this.num].name + ' getoond worden.');
                  console.log('Er kon geen voorbeeld van het bestand ' + files[this.num].name + ' getoond worden.');
                }
              });
        }else if(['jpeg','jpg','png','gif','bmp'].indexOf(extension) >= 0){
          t.setState({ imagesContent: t.state.imagesContent + "<img src='"+this.result+"' alt='preview' style='max-width: 100%;' />" });
        }else if(['avi','mp4','mpeg', 'webm'].indexOf(extension) >= 0){
          t.setState({ imagesContent: t.state.imagesContent + "<video controls style='max-width: 100%;width: 100%;'><source src='"+this.result+"' /></video>" });
        }else if(['mp3','wav'].indexOf(extension) >= 0){
          t.setState({ imagesContent: t.state.imagesContent + "<audio controls style='max-width: 100%;width: 100%;'><source src='"+this.result+"' /></audio>" });
        }else{
          console.log('Er kon geen voorbeeld van het bestand ' + files[this.num].name + ' getoond worden.');
          toastr.warning('Er kon geen voorbeeld van het bestand ' + files[this.num].name + ' getoond worden.');
        }

      };
      reader.onerror = function (error) {
        console.log('Error: ', error);
        toastr.warning('Er is iets fout gedaan. Probeer het a.u.b opnieuw.')
      };
    }
    // Set files state
    this.setState({
      files: files
    });
  }

  togglePopup1() {
    this.setState({
      showPopup1: !this.state.showPopup1
    });
  }

  togglePopup2() {
    this.setState({
      showPopup2: !this.state.showPopup2
    });
  }

  togglePopup3() {
    this.setState({
      showPopup3: !this.state.showPopup3
    });
  }

  togglePopup4() {
    this.setState({
      showPopup4: !this.state.showPopup4
    });
  }

  editorHandler(data){
    this.setState({
      editorContent: data
    });
  }

  insertItem(e) {
    e.preventDefault();

    var formData = new FormData();
    formData.append("title", createItemForm.title.value);
    formData.append("icon", createItemForm.item.value);
    formData.append("texts[]", this.state.editorContent);
    var filesInput = this.state.files;

    if(filesInput){
      for(var i=0;i<filesInput.length;i++){
        formData.append("files[]", filesInput[i]);
      }
    }

    fetch('/api/v1/story', {
        method: 'POST',
        body: formData
    }).then(response => response.json())
      .then(response => {
        console.log(response);

        if(response['response'] == "success"){
          toastr.success('Het item is toegevoegd!');
          window.location.href = "/overview";
        }else{
          toastr.warning('Er is iets fout gedaan. Probeer het a.u.b opnieuw.')
        }
    });
  }

  render() {
    const isChecked = {
      display: this.state.isChecked ? "block" : "none"
    };
    
    const hidden = {
      display: this.state.isChecked ? "none" : "block"
    }

    return (
    <div className="container-fluid containerAddItem">

      <div className="col s12 m5 l5 inputFields">
        <form name="createItemForm" autocomplete="off" encType="multipart/form-data">

        <div className="row">
          <div className="input-field col s11">
            <input id="title" name="title" className="inputField" type="text" required onChange={this.changeState.bind(this)}></input>
            <label htmlFor="title">Titel</label>
          </div>
          <div className="question questionTitle col s1">
            <FontAwesomeIcon icon={ faQuestionCircle } onClick={this.togglePopup1.bind(this)} />
          </div>
        </div>
          {this.state.showPopup1 ?  
            <Popup  title={this.state.text[0].title} closePopup={this.togglePopup1.bind(this)}/>  
            : null  
          }  

            <label className="label" htmlFor="existingFile">Bestaand document</label>
            <div className="row">
              <div className="switch existingFile col s11">
                <label class="label">
                  Nee
                  <input type="checkbox" name="existingFile" onChange={ this.changeStateSwitch.bind(this) } checked={ this.state.isChecked }></input>
                  <span className="lever"></span>
                  Ja
                </label>
              </div>
              <div className="question col s1">
                <FontAwesomeIcon icon={ faQuestionCircle } onClick={this.togglePopup2.bind(this)}/>
              </div>
            </div>

            {this.state.showPopup2 ?
            <Popup  title={this.state.text[1].title} closePopup={this.togglePopup2.bind(this)}/>
            : null
            }

            <div className="row" style={ hidden }>
              <div className="input-field col s11">
                <CKEditor
                    editor={ ClassicEditor }
                    data=" "
                    config={ {
                        toolbar: [ [ 'Heading' ], [ 'Bold' ], [ 'Italic' ], ['imageUpload'] ],
                        extraPlugins: [ this.customAdapterPlugin ]
                    } }
                    onChange={ ( event, editor ) => {
                      const data = editor.getData();
                      this.editorHandler(data)
                    } }
                />
              </div>
              <div className="question questionOwnText col s1">
                <FontAwesomeIcon icon={ faQuestionCircle } onClick={this.togglePopup3.bind(this)} />
              </div>
            </div>

        {this.state.showPopup3 ?  
        <Popup  title={this.state.text[2].title} closePopup={this.togglePopup3.bind(this)}/>  
        : null  
        }  

        <div className="row" style={ isChecked }>
          <div className="input-field col s11">
            <div className="file-field input-field">
              <div className="btn">
                <span>Bestand</span>
                <input name="uploadFileButton" onChange={this.setFile.bind(this)} type="file" multiple></input>
              </div>
              <div className="file-path-wrapper">
                <input className="file-path validate" name="uploadFile" type="text" ></input>
              </div>
            </div>
          </div>
          <div className="question questionExFile col s1">
            <FontAwesomeIcon icon={ faQuestionCircle } onClick={this.togglePopup4.bind(this)} />
          </div>
        </div>

        {this.state.showPopup4 ?  
        <Popup  title={this.state.text[3].title} closePopup={this.togglePopup4.bind(this)}/>  
        : null  
        }  

        <label class="label" htmlFor="chooseIcon">Kies een icoon</label>
        <div className="row col s12 itemCollection">

        <label htmlFor="item1">
          <div className="items">
            <input className="with-gap" id="item1" name="item" value="rat" type="radio" defaultChecked></input>
            <span className="itemImage"><img src={ require('../../../public/images/rat2.png') } /></span>
          </div>
        </label>

        <label htmlFor="item2">
          <div className="items">
            <input className="with-gap" id="item2" name="item" value="envelope" type="radio"></input>
            <span className="itemImage"><img src={ require('../../../public/images/envelope2.png') } /></span>
          </div>
        </label>

          <label htmlFor="item3">
            <div className="items">
              <input className="with-gap" id="item3" name="item" value="candle" type="radio"></input>
              <span className="itemImage"><img src={ require('../../../public/images/candle2.png') } /></span>
            </div>
          </label>

          <label htmlFor="item4">
            <div className="items">
              <input className="with-gap" id="item4" name="item" value="binoculars" type="radio"></input>
              <span className="itemImage"><img src={ require('../../../public/images/binocular2.png') } /></span>
            </div>
          </label>

          <label htmlFor="item5">
            <div className="items">
              <input className="with-gap" id="item5" name="item" value="crownsPen" type="radio"></input>
              <span className="itemImage"><img src={ require('../../../public/images/kroontjesPen2.png') } /></span>
            </div>
          </label>

          <label htmlFor="item6">
            <div className="items">
              <input className="with-gap" id="item6" name="item" value="yat" type="radio"></input>
              <span className="itemImage"><img src={ require('../../../public/images/yatvasum2.png') } /></span>
            </div>
          </label>

        </div>

        <div className="row">
          <div className="input-field col s4">
            <button className="btn waves-effect waves-light saveButton" type="submit" onClick={this.insertItem.bind(this)} name="action">Opslaan</button>
          </div>
        </div>

        </form>
      </div>


      <div className="col s12 m7 l7 exampleSide">

        <div className="row exampleCard">
          <div className="allContent">
            <div className="title">
              <p> {this.state.title} </p>
            </div>
            <div className="content">
              <p dangerouslySetInnerHTML={{__html: this.state.editorContent}} style={hidden} />
              <p dangerouslySetInnerHTML={{__html: this.state.imagesContent}} style={isChecked}/>
            </div>
            <div className="row feedback">
              <p>Welke emotie wekte dit verhaal bij jou op?</p>
              <div className="col s3">
                <span className="buttonsFeedback"><img src={ require('../../../public/images/poststamp2.png') } /></span>
              </div>
              <div className="col s3">
                <span className="buttonsFeedback"><img src={ require('../../../public/images/poststamp2.png') } /></span>
              </div>
              <div className="col s3">
                <span className="buttonsFeedback"><img src={ require('../../../public/images/poststamp2.png') } /></span>
              </div>
            </div>

          </div>
        </div>
      </div>
    </div>
    )
  }
}

export default CreateItem;