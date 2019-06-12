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
    this.state.modal = M.Modal.init(document.getElementById('story_example'), {opacity: 0});
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
    this.setState({
      modalContent: "<iframe class=\"modal-container\" src=\"/api/v1/story/"+id+"/preview\" frameborder=\"0\"></iframe>"
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
          <div className="modal-content" dangerouslySetInnerHTML={{__html: this.state.modalContent}}>

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