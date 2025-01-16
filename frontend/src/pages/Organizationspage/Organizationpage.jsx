import { useEffect, useState } from 'react';
import {
    getCodByName,
    getCodByTag,
    getCODS,
} from '../../services/organizations.service';
import Loader from '../../components/Loader/Loader';
import Dropdown from '../../components/Dropdown/Dropdown';
import Input from '../../components/UI/Input/Input';
import Button from '../../components/UI/Button/Button';

const Organizationspage = () => {
    const [isLoading, setIsLoading] = useState(false);
    const [data, setData] = useState();
    const [name, setName] = useState();
    const [tag, setTag] = useState();
    const [searchClick, setSearchClick] = useState({
        type: null,
        content: '',
        isClicked: false,
    });
    console.log(data);
    const fetchCods = () => {
        setIsLoading(true);
        getCODS()
            .then((res) => {
                if (res.status === 200) return res.json();
                return null;
            })
            .then((data) => {
                setData(data);
            })
            .finally(() => {
                setIsLoading(false);
                setSearchClick((prev) => ({
                    ...prev,
                    isClicked: false,
                }));
            });
    };
    const fetchCodByName = () => {
        setIsLoading(true);
        getCodByName(name)
            .then((res) => {
                if (res.status === 200) return res.json();
                return null;
            })
            .then((data) => setData(data))
            .finally(() => {
                setIsLoading(false);
                setSearchClick((prev) => ({
                    ...prev,
                    isClicked: false,
                }));
            });
    };
    const fetchCodByTag = () => {
        setIsLoading(true);
        getCodByTag(tag)
            .then((res) => {
                if (res.status === 200) return res.json();
                return null;
            })
            .then((data) => setData(data))
            .finally(() => {
                setIsLoading(false);
                setSearchClick((prev) => ({
                    ...prev,
                    isClicked: false,
                }));
            });
    };
    useEffect(() => {
        fetchCods();
    }, []);
    useEffect(() => {
        if (searchClick.isClicked) {
            if (!searchClick.content) {
                fetchCods();
            } else if (searchClick.type === 'name') {
                fetchCodByName();
            } else {
                fetchCodByTag();
            }
        }
    }, [searchClick.isClicked]);

    return (
        <div>
            <div className="flex flex-col gap my">
                <div className="form">
                    <Input
                        placeholder={'Поиск по имени:'}
                        onChange={(e) => setName(e.target.value)}
                    />
                    <Button
                        onClick={() =>
                            setSearchClick({
                                type: 'name',
                                content: name,
                                isClicked: true,
                            })
                        }
                    >
                        Search
                    </Button>
                </div>

                <div className="form">
                    <Input
                        placeholder={'Поиск по vlanTag:'}
                        onChange={(e) => setTag(e.target.value)}
                    />
                    <Button
                        onClick={() => {
                            setSearchClick({
                                type: 'tag',
                                content: tag,
                                isClicked: true,
                            });
                        }}
                    >
                        Search
                    </Button>
                </div>
            </div>
            {isLoading ? <Loader /> : null}
            {data ? (
                data.map((organization) => (
                    <div
                        key={organization?.idClient}
                        className="betonen"
                        style={{ borderColor: 'white' }}
                    >
                        <div>Id: {organization?.idClient}</div>
                        <div>Name: {organization?.name}</div>
                        <Dropdown
                            activeText={'show info'}
                            hideText={'hide info'}
                        >
                            <div
                                style={{
                                    paddingLeft: '1rem',
                                }}
                            >
                                <div>contactC: {organization?.contactC}</div>
                                <div>
                                    telephoneC: {organization?.telephoneC}
                                </div>
                                <div>contactT: {organization?.contactT}</div>
                                <div>
                                    telephoneT: {organization?.telephoneT}
                                </div>
                                <div>emailC: {organization?.emailC}</div>
                                <div>
                                    working: {organization?.working.toString()}
                                </div>
                                <div>emailT: {organization?.emailT}</div>
                                <div>history: {organization?.history}</div>
                                <div>
                                    antiDDOS:{' '}
                                    {organization?.antiDDOS.toString()}
                                </div>
                                <Dropdown
                                    activeText={'show cod'}
                                    hideText={'hide cod'}
                                >
                                    <div className="betonen">
                                        <div>
                                            id_COD: {organization?.id_COD}
                                        </div>

                                        <div>
                                            nameCOD:{' '}
                                            {organization?.cod?.nameCOD}
                                        </div>
                                        <div>
                                            telephone:{' '}
                                            {organization?.cod?.telephone}
                                        </div>
                                        <div>
                                            email1: {organization?.cod?.email1}
                                        </div>
                                        <div>
                                            email2: {organization?.cod?.email2}
                                        </div>
                                        <div>
                                            contact:{' '}
                                            {organization?.cod?.contact}
                                        </div>
                                        <div>
                                            description:{' '}
                                            {organization?.cod?.description}
                                        </div>
                                        <div>
                                            region: {organization?.cod?.region}
                                        </div>
                                    </div>
                                </Dropdown>

                                <Dropdown
                                    activeText={'show tfPlan'}
                                    hideText={'hide tfPlan'}
                                >
                                    <div className="betonen">
                                        <div>
                                            id_TPlan: {organization?.id_TPlan}
                                        </div>

                                        <div>
                                            nameTfPlan:{' '}
                                            {organization?.tfPlan?.nameTfPlan}
                                        </div>
                                        <div>
                                            descTfPlan:{' '}
                                            {organization?.tfPlan?.descTfPlan}
                                        </div>
                                    </div>
                                </Dropdown>
                                {organization?.sprVlans?.length ? (
                                    <div>
                                        <Dropdown
                                            activeText={'show sprvlans'}
                                            hideText={'hide sprvlans'}
                                        >
                                            <div>
                                                {organization?.sprVlans?.map(
                                                    (lan) => (
                                                        <div
                                                            key={lan?.idVlan}
                                                            className="betonen"
                                                        >
                                                            <div>
                                                                idVlan:{' '}
                                                                {lan?.idVlan}
                                                            </div>
                                                            <div>
                                                                idClient:{' '}
                                                                {lan?.idClient}
                                                            </div>
                                                            <div>
                                                                useClient:{' '}
                                                                {lan?.useClient.toString()}
                                                            </div>
                                                            <div>
                                                                useCOD:{' '}
                                                                {lan?.useCOD.toString()}
                                                            </div>
                                                        </div>
                                                    )
                                                )}
                                            </div>
                                        </Dropdown>
                                    </div>
                                ) : null}
                            </div>
                        </Dropdown>
                    </div>
                ))
            ) : !isLoading ? (
                <div style={{ fontSize: '2rem' }}>Not found</div>
            ) : null}
        </div>
    );
};

export default Organizationspage;
