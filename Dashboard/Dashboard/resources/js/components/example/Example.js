import React from 'react';
import { faTimes } from '@fortawesome/free-solid-svg-icons';

class Popup extends React.Component {
    render() {
        return (
            <div className='popup'>
                <div className='popup_inner row'>
                <div className='col s11'>
                    <p>Hier kan de titel ingevoerd worden die terug te vinden is boven dit item.</p>
                </div>
                <div className='cross col s1'>
                
                </div>
                </div>
            </div>
        )}
    }

    export default Popup;