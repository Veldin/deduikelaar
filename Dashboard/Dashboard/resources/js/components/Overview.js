import React, {Component} from 'react';
import toastr from 'toastr';
import Card from './Card/Card';
import {FontAwesomeIcon} from "@fortawesome/react-fontawesome";
import { faSpinner } from "@fortawesome/free-solid-svg-icons";

class Overview extends Component {
  constructor() {
    super();

    this.state = {
      card: [],
      switch: true,
      modal: null,
      modalContent: ""
    }
  }

  componentDidMount() {
    this.state.modal = M.Modal.init(document.getElementById('story_example'), {});
    fetch('/api/v1/overview')
      .then((response) => response.json())
      .then((responseJson) => {
        this.setState({
          card: responseJson
        })
      })
  }

  toggleSwitch() {
    this.setState({ 
      switch: !this.state.switch
    })
    console.log(this.state.switch)
  }

  deleteItem(id) {
    fetch('/api/v1/story/'+id,{
      method: 'DELETE',
    })
    .then(response => response.json())
    .then(response => {
      if(response['response'] == "success"){
        toastr.success('Het item is verwijderd!', '', {positionClass: "toast-bottom-right", timeOut: 40000})
        this.setState({
          card: this.state.card.filter(s => s.storyId !== id)
        });
      }else{
        toastr.warning('dit item is al verwijderd', '', {positionClass: "toast-bottom-right", timeOut: 40000})
      }
    })
  }

  showItem(id) {

    fetch('/api/v1/story/'+id+'/preview')
    .then(response => response.json())
    .then(response => {
      if(response['response'] === "success"){
        this.setState({
          modalContent: response['data']
        });
      }else{
        this.state.modal.close();
      }
    });
    this.setState({
      modalContent: '<svg aria-hidden="true" focusable="false" data-prefix="fas" data-icon="spinner" class="svg-inline--fa fa-spinner fa-w-32 fa-spin fa-pulse " role="img" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 512 512"><path fill="currentColor" d="M304 48c0 26.51-21.49 48-48 48s-48-21.49-48-48 21.49-48 48-48 48 21.49 48 48zm-48 368c-26.51 0-48 21.49-48 48s21.49 48 48 48 48-21.49 48-48-21.49-48-48-48zm208-208c-26.51 0-48 21.49-48 48s21.49 48 48 48 48-21.49 48-48-21.49-48-48-48zM96 256c0-26.51-21.49-48-48-48S0 229.49 0 256s21.49 48 48 48 48-21.49 48-48zm12.922 99.078c-26.51 0-48 21.49-48 48s21.49 48 48 48 48-21.49 48-48c0-26.509-21.491-48-48-48zm294.156 0c-26.51 0-48 21.49-48 48s21.49 48 48 48 48-21.49 48-48c0-26.509-21.49-48-48-48zM108.922 60.922c-26.51 0-48 21.49-48 48s21.49 48 48 48 48-21.49 48-48-21.491-48-48-48z"></path></svg>'
    });
    this.state.modal.open();
  }



  render(){
    return (
      <div className="overviewContentContainer">
        <div className="row overviewFilter">
          <div className="col s4 m2 l2 overviewLabel">Alle Items</div>
          <div className="col s8 m10 l10 overviewSwitch">
            <div className="switch" onChange={this.toggleSwitch.bind(this)}>
              <label>
                Alleen Actief
                <input type="checkbox" defaultChecked></input>
                <span className="lever"></span>
                Alle
              </label>
            </div>
          </div>
        </div>
        <div className="row overviewCards">
          <div className="cards-container">
            {this.state.card.map((item, key) =>
              <Card 
                onDelete={this.deleteItem.bind(this)}
                onShow={this.showItem.bind(this)}
                key={key} 
                active={item.active}
                storyID={item.storyId} 
                title={item.title} 
                active={item.active}
                cardInfo={[this.state.card]}
              >
              </Card>
            )}
          </div>
        </div>
        <div id="story_example" className="modal modal-fixed-footer preview">
          <div className="modal-content">
            <div className="modal-container" dangerouslySetInnerHTML={{__html: this.state.modalContent}}>
            </div>
          </div>
          <div className="modal-footer">
            <a href="#!" className="modal-close btn waves-effect waves-light btn-flat">Sluit</a>
          </div>
        </div>
      </div>
    )
  }
}
export default Overview;