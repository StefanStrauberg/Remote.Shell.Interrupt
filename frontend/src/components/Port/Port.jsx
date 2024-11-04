import Wrapper from '../Wrapper/Wrapper';
import classes from './Port.module.css';
import Item from '../../pages/Gatewaypage/components/Item/Item';
import { PORT } from '../../data/port';
import Dropdown from '../Dropdown/Dropdown';
export const Port = ({ port }) => {
    return (
        <div className={classes.port}>
            <Wrapper>
                <div className={classes.routerInfoWrapper}>
                    <div className={classes.routerInfoTitle}>
                        {port.interfaceName}
                    </div>
                    <div className={classes.routerInfoDescr}>
                        {PORT.map(({ title, key }) => (
                            <Item title={title} descr={port[key]} key={key} />
                        ))}
                        {port.vlaNs.length ? (
                            <Dropdown
                                activeText={'show vlan'}
                                hideText={'hide vlans'}
                            >
                                {port.vlaNs.map((lan) => (
                                    <div key={lan.vlanName}>
                                        <div className={classes.lan}>
                                            <div>vlan tag: ${lan.vlanTag}</div>
                                            <div>
                                                vlan name: ${lan.vlanName}
                                            </div>
                                        </div>
                                    </div>
                                ))}
                            </Dropdown>
                        ) : null}
                        <div style={{ marginTop: '2rem', marginLeft: '2rem' }}>
                            {port.isAggregated
                                ? port.aggregatedPorts.map((port) => (
                                      <div key={port.id}>
                                          <div
                                              className={`${classes.routerInfoTitle}`}
                                          >
                                              {port.interfaceName}
                                          </div>
                                          <div
                                              className={classes.aggregatedPort}
                                          >
                                              {PORT.map(({ title, key }) => (
                                                  <Item
                                                      title={title}
                                                      descr={port[key]}
                                                      key={key}
                                                  />
                                              ))}
                                              {port.vlaNs.length ? (
                                                  <Dropdown
                                                      activeText={'show vlan'}
                                                      hideText={'hide vlans'}
                                                  >
                                                      {port.vlaNs.map((lan) => (
                                                          <div
                                                              key={lan.vlanName}
                                                          >
                                                              <div
                                                                  className={
                                                                      classes.lan
                                                                  }
                                                              >
                                                                  <div>
                                                                      vlan tag:
                                                                      $
                                                                      {
                                                                          lan.vlanTag
                                                                      }
                                                                  </div>
                                                                  <div>
                                                                      vlan name:
                                                                      $
                                                                      {
                                                                          lan.vlanName
                                                                      }
                                                                  </div>
                                                              </div>
                                                          </div>
                                                      ))}
                                                  </Dropdown>
                                              ) : null}
                                          </div>
                                      </div>
                                  ))
                                : null}
                        </div>
                    </div>
                </div>
            </Wrapper>
        </div>
    );
};
