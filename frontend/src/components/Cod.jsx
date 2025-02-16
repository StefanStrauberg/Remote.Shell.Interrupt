import Item from '../pages/Gatewaypage/components/Item/Item';
import Button from './UI/Button/Button';
import Wrapper from './Wrapper/Wrapper';
import classes from '../pages/Gatewayspage/Gatewayspage.module.css';
import { useState } from 'react';
import Dropdown from './Dropdown/Dropdown';
import styles from '../components/Port/Port.module.css';

const Cod = ({ cod }) => {
    const [isActive, setIsActive] = useState(false);

    return (
        <>
            <Wrapper>
                <div className="!text-2xl">Абонент:</div>
                <div className="flex gap-4 justify-between">
                    <div>
                        <div className="!text-xl">{cod?.name}</div>{' '}
                        <div className="!text-xl">{cod?.contactT}</div>{' '}
                        <div className="flex flex-wrap !text-xl">
                            ({cod?.telephoneT} {cod?.emailC})
                        </div>
                    </div>
                    <div>
                        <Button
                            isCustom={true}
                            onClick={() => setIsActive((prev) => !prev)}
                        >
                            {' '}
                            {isActive ? 'Скрыть' : 'Подробнее'}
                        </Button>
                    </div>
                </div>
            </Wrapper>
            {isActive ? (
                <div className="mt-4">
                    <Wrapper>
                        <div className={'!text-3xl !font-bold'}>
                            Общая информация
                        </div>
                        <div className={classes.routerInfoDescr}>
                            <Item title={'имя COD'} descr={cod?.cod?.nameCOD} />
                            <Item
                                title={'телефон'}
                                descr={cod?.cod?.telephone}
                            />
                            <Item title={'почта 1'} descr={cod?.cod?.email1} />
                            <Item title={'почта 2'} descr={cod?.cod?.email2} />
                            <Item title={'контакт'} descr={cod?.cod?.contact} />
                            <Item
                                title={'описание'}
                                descr={cod?.cod?.description}
                            />
                            <Item title={'регион'} descr={cod?.cod?.region} />
                        </div>
                    </Wrapper>
                    <div className="mt-4">
                        <Wrapper>
                            {' '}
                            <div className={'!text-3xl !font-bold'}>
                                Тарифный план
                            </div>
                            <div className={classes.routerInfoDescr}>
                                <Item
                                    title={'Название'}
                                    descr={cod?.tfPlan?.nameTfPlan}
                                />
                                <Item
                                    title={'Описание'}
                                    descr={cod?.tfPlan?.descTfPlan}
                                />
                            </div>
                        </Wrapper>

                        <div className="mt-4">
                            <Wrapper>
                                <Item title={'Показать теги'} descr={''} />
                                <Dropdown
                                    activeText={'show vlan'}
                                    hideText={'hide vlans'}
                                >
                                    {cod?.sprVlans?.map((lan) => (
                                        <div key={lan.idVlan + Math.random()}>
                                            <div className={styles.lan}>
                                                <div>vlan id: {lan.idVlan}</div>
                                                <div>
                                                    client id: {lan.idClient}
                                                </div>
                                                <div>
                                                    useClient:{' '}
                                                    {String(lan.useClient)}
                                                </div>
                                                <div>
                                                    useCOD: {String(lan.useCOD)}
                                                </div>
                                            </div>
                                        </div>
                                    ))}
                                </Dropdown>
                            </Wrapper>
                        </div>
                    </div>
                </div>
            ) : null}
        </>
    );
};

export default Cod;
