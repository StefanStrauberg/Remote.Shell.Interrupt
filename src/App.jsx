import { Route, Routes } from 'react-router-dom';
import Gatewayspage from './pages/Gatewayspage/Gatewaypage';
import MainLayout from './Layouts/MainLayout';
import Gatewaypage from './pages/Gatewaypage/Gatewaypage';
import Assigmentspage from './pages/Assigmentspage/Assigmentspage';
import Form from './components/UI/Form/Form';
import Rulespage from './pages/Rulespage/Rulespage';

function App() {
    return (
        <>
            <Routes>
                <Route path="/" element={<MainLayout />}>
                    <Route index element={<Gatewayspage />} />
                    <Route path="/gateways/:id" element={<Gatewaypage />} />
                    <Route
                        path="/gateways/create"
                        element={
                            <Form types={[{ name: 'IP', input: 'input' }]} />
                        }
                    />
                    <Route path="/assigments" element={<Assigmentspage />} />

                    <Route
                        path="/assigments/create"
                        element={
                            <Form
                                types={[
                                    { name: 'Name', input: 'input' },
                                    { name: 'Type of request', input: 'input' },
                                    {
                                        name: 'Target field name',
                                        input: 'input',
                                    },
                                    { name: 'OID', input: 'input' },
                                ]}
                            />
                        }
                    ></Route>
                    <Route
                        path="/assigments/edit"
                        element={
                            <Form
                                types={[
                                    { name: 'Name', input: 'input' },
                                    { name: 'Type of request', input: 'input' },
                                    {
                                        name: 'Target field name',
                                        input: 'input',
                                    },
                                    { name: 'OID', input: 'input' },
                                ]}
                            />
                        }
                    ></Route>
                    <Route path="/rules" element={<Rulespage />} />
                    <Route
                        path="/rules/edit"
                        element={
                            <Form
                                types={[
                                    { name: 'Name', input: 'input' },
                                    { name: 'Condition', input: 'input' },
                                    { name: 'Assigment?', input: 'select' },
                                ]}
                            />
                        }
                    />
                    <Route
                        path="/rules/create"
                        element={
                            <Form
                                types={[
                                    { name: 'Name', input: 'input' },
                                    { name: 'Condition', input: 'input' },
                                    { name: 'Assigment?', input: 'select' },
                                ]}
                            />
                        }
                    />
                    <Route path="*" />
                </Route>
            </Routes>
        </>
    );
}

export default App;
