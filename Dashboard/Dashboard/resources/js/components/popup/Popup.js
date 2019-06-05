import React from 'react';
import { faTimes } from '@fortawesome/free-solid-svg-icons';

const Popup = (props) =>{

        return (
            <div className='popup'>
                <div className='popup_inner row'>
                <div className='col s11'>
                    <p>{props.title}</p>
                </div>
                <div className='cross col s1'>
                    
                </div>
                </div>
            </div>
        )
    }

    export default Popup;