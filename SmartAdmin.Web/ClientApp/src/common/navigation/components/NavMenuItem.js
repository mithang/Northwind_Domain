import React from 'react'
import PropTypes from 'prop-types'
import { NavLink } from 'react-router-dom';

import {Msg} from '../../i18n'

import SmartMenuList from './NavMenuList'


export default class SmartMenuItem extends React.Component {

  static contextTypes = {
    router: PropTypes.object.isRequired
  };

  render() {
    const item = this.props.item;


    const title = !item.parent ?
      <span className="menu-item-parent"><Msg phrase={item.title}/></span> :
      <Msg phrase={item.title}/>;

    const badge = item.badge ? <span className={item.badge.class}>{item.badge.label || ''}</span> : null;
    const childItems = item.items ? <SmartMenuList items={item.items}/> : null;

    const icon = item.icon ? (
      item.counter ? <i className={item.icon}><em>{item.counter}</em></i> : <i className={item.icon}/>
    ) : null;


    const liClassName = isItemActive(item, this.context.router.route) ? 'active' : '';

    const link = item.route ?
      <NavLink to={item.route} title={item.title} activeClassName="active">
        {icon} {title} {badge}
      </NavLink> :
      <a href={item.href || '#'} onClick={this._handleClick} title={item.title}>
        {icon} {title} {badge}
      </a>;


    return <li className={liClassName}>{link}{childItems}</li>
  }
}


function isItemActive(item, route){
  if(item.route){
    return item.route === route.location.pathname
  } else if (item.items) {  
    return item.items.some(_ => isItemActive(_, route))
  } else {
    return false
  }
}